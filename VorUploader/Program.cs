// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Client;
using System.Globalization;
using System.Net.Http.Json;
using System.Reflection;

namespace VorUploader;

/// <summary>
/// Main program class.
/// </summary>
internal static class Program
{
    private static async Task Main()
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<VorIncident>()
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var baseUri = new Uri(configuration["BaseUri"] ?? throw new InvalidOperationException("BaseUri not set in configuration."));

        var services = new ServiceCollection();
        services.AddOpenIddict()
            .AddClient(o =>
            {
                o.AllowClientCredentialsFlow();
                o.DisableTokenStorage();
                o.UseSystemNetHttp().SetProductInformation(typeof(Program).Assembly);
                o.AddRegistration(new OpenIddict.Client.OpenIddictClientRegistration
                {
                    Issuer = baseUri,
                    ClientId = configuration["OpenIdWorkerSettings:VorUploaderClientId"],
                    ClientSecret = configuration["OpenIdWorkerSettings:VorUploaderClientSecret"],
                });
            });

        await using var provider = services.BuildServiceProvider();

        var token = await GetTokenAsync(provider);

        using var httpClient = new HttpClient()
        {
            BaseAddress = baseUri,
        };

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var vorUris = new Uri("/api/vor", UriKind.Relative);

        foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.xls").OrderBy(f => f))
        {
            Console.WriteLine($"Found File - {file}");

            var fileDate = DateOnly.Parse(Path.GetFileNameWithoutExtension(file).Split(' ')[0], CultureInfo.CurrentCulture);

            Console.WriteLine($"File Date  - {fileDate}");

            var items = FileParser.ParseFile(file, fileDate);

            var count = 0;
            HttpResponseMessage result;

            do
            {
                if (count > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                result = await httpClient.PostAsJsonAsync(vorUris, items);
                count++;
            }
            while (count < 3 && !result.IsSuccessStatusCode);

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine($"Received Error : {result.StatusCode}");
            }
            else
            {
                var dirPath = Path.GetDirectoryName(file) ?? Environment.CurrentDirectory;
                Directory.CreateDirectory(Path.Combine(dirPath, "Uploaded"));
                File.Move(file, Path.Combine(dirPath, "Uploaded", Path.GetFileName(file)), true);
            }

        }

        await CancelToken(provider, token);
    }

    static async Task<string> GetTokenAsync(IServiceProvider provider)
    {
        var service = provider.GetRequiredService<OpenIddictClientService>();

        var result = await service.AuthenticateWithClientCredentialsAsync(new());
        return result.AccessToken;
    }

    static async Task CancelToken(IServiceProvider provider, string token)
    {
        var service = provider.GetRequiredService<OpenIddictClientService>();

        await service.RevokeTokenAsync(new() { Token = token });
    }
}
