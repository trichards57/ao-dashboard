// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Azure.Identity;
using Dashboard.Grpc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using OpenIddict.Client;
using System.Reflection;

namespace VorUploader;

/// <summary>
/// Main program class.
/// </summary>
internal static class Program
{
    private const string InboxId = "AAMkAGU0NTA2ODczLTYyZWEtNGZhNS04MDQ0LTIwMDFlMDQzN2M0ZQAuAAAAAAB20NTeTHozS4sqw68MW0ExAQBWg0hGD4wFSaUCnLIUVcwOAAAA-DKkAAA=";
    private const string TokenReplyString = "http://localhost/";
    private const string VorFolderId = "AAMkAGU0NTA2ODczLTYyZWEtNGZhNS04MDQ0LTIwMDFlMDQzN2M0ZQAuAAAAAAB20NTeTHozS4sqw68MW0ExAQD7P9wa3zxsSruC4g1eWMZTAATImwK9AAA=";

    private static async Task CancelToken(IServiceProvider provider, string token)
    {
        var service = provider.GetRequiredService<OpenIddictClientService>();

        await service.RevokeTokenAsync(new() { Token = token });
    }

    private static async Task<string> GetTokenAsync(IServiceProvider provider)
    {
        var service = provider.GetRequiredService<OpenIddictClientService>();

        var result = await service.AuthenticateWithClientCredentialsAsync(new());
        return result.AccessToken;
    }

    private static async Task Main()
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<VorIncident>(true)
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
                o.AddRegistration(new OpenIddictClientRegistration
                {
                    Issuer = baseUri,
                    ClientId = configuration["OpenIdWorkerSettings:VorUploaderClientId"],
                    ClientSecret = configuration["OpenIdWorkerSettings:VorUploaderClientSecret"],
                });
            });

        await using var provider = services.BuildServiceProvider();

        // Get AO Dashboard Token
        var token = await GetTokenAsync(provider);

        using var httpClient = new HttpClient()
        {
            BaseAddress = baseUri,
        };

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var headers = new Metadata();
        headers.Add("Authorization", $"Bearer {token}");

        var vorUris = new Uri("/api/vor", UriKind.Relative);

        // Get a Microsoft Graph token
        var scopes = new[] { "User.Read", "Mail.ReadWrite" };
        var graphClientId = configuration["Authentication:Microsoft:ClientId"];
        var tenantId = "91d037fb-4714-4fe8-b084-68c083b8193f";

        var options = new InteractiveBrowserCredentialOptions()
        {
            TenantId = tenantId,
            ClientId = graphClientId,
            RedirectUri = new Uri(TokenReplyString),
            LoginHint = "tony.richards1@sja.org.uk",
        };

        var interactiveCredential = new InteractiveBrowserCredential(options);

        using var graphClient = new GraphServiceClient(interactiveCredential, scopes);

        var emails = await graphClient.Me.MailFolders[InboxId].Messages.GetAsync(x =>
        {
            x.QueryParameters.Top = 100;
        });

        var grpcClient = new Vor.VorClient(GrpcChannel.ForAddress(baseUri));

        if (emails?.Value != null)
        {
            foreach (var e in emails.Value.Where(e => e.Subject?.Equals("Daily VOR Report", StringComparison.OrdinalIgnoreCase) == true))
            {
                var attachments = await graphClient.Me.Messages[e.Id].Attachments.GetAsync();
                var attachment = attachments?.Value?.Find(a => a.Name?.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) == true);

                if (attachment == null
                    || e.ReceivedDateTime == null
                    || attachment is not FileAttachment fa
                    || fa.ContentBytes == null)
                {
                    continue;
                }

                var fileDate = System.DateOnly.FromDateTime(e.ReceivedDateTime.Value.Date);
                Console.WriteLine($"Found File - Date: {e.ReceivedDateTime}");

                var tempFile = Path.GetRandomFileName();
                await File.WriteAllBytesAsync(tempFile, fa.ContentBytes);

                var items = FileParser.ParseFile(tempFile, fileDate);

                var count = 0;
                AddVorIncidentResponse result;

                do
                {
                    if (count > 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }

                    var req = new AddVorIncidentRequest();
                    req.Incidents.AddRange(items);

                    result = await grpcClient.AddIncidentAsync(req, headers);

                    count++;
                }
                while (count < 3 && !result.Success);

                if (!result.Success)
                {
                    Console.WriteLine("Received Error.");
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Uploaded"));
                    File.Move(tempFile, Path.Combine(Environment.CurrentDirectory, "Uploaded", $"{fileDate:o} VOR Report.xls"), true);
                    // await graphClient.Me.Messages[e.Id].Move.PostAsync(new Microsoft.Graph.Me.Messages.Item.Move.MovePostRequestBody { DestinationId = VorFolderId });
                }
            }
        }

        await CancelToken(provider, token);
    }
}
