﻿// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Globalization;
using System.Reflection;

namespace AODashboard.VorUploader;

/// <summary>
/// Main program class.
/// </summary>
internal static class Program
{
    private static readonly string[] Scopes = ["openid", "api://ae7dee55-3f98-4bda-b5cf-7641de4a1776/VOR.Write"];

    private static async Task Main()
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var brokerOptions = new BrokerOptions(BrokerOptions.OperatingSystems.Windows)
        {
            Title = "AO Dashboard VOR Uploader",
        };

        var storageProperties = new StorageCreationPropertiesBuilder(configuration["Authentication:CacheFileName"], Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VorUploader")).Build();

        var client = PublicClientApplicationBuilder
            .Create(configuration["AzureAD:ClientId"])
            .WithTenantId(configuration["AzureAD:TenantId"])
            .WithParentActivityOrWindow(ConsoleFunctions.GetConsoleOrTerminalWindow)
            .WithBroker(brokerOptions)
            .Build();

        var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
        cacheHelper.RegisterCache(client.UserTokenCache);

        AuthenticationResult? token = null;

        var accountToLogin = (await client.GetAccountsAsync()).FirstOrDefault(a => a.Username.Contains("sja.org.uk"));

        if (accountToLogin != null)
        {
            try
            {
                token = await client.AcquireTokenSilent(Scopes, accountToLogin).ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                token = null;
            }
        }

        token ??= await client.AcquireTokenInteractive(Scopes).ExecuteAsync();

        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri(configuration["BaseUri"]!),
        };

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

        foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.xls").OrderBy(f => f))
        {
            Console.WriteLine($"Found File - {file}");

            var fileDate = DateOnly.Parse(Path.GetFileNameWithoutExtension(file).Split(' ')[0], CultureInfo.CurrentCulture);

            Console.WriteLine($"File Date  - {fileDate}");

            if (fileDate == DateOnly.FromDateTime(DateTime.Now.Date))
            {
                var result = await httpClient.DeleteAsync("/api/vors");

                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Could not clear current VORs.  {result.StatusCode} - {result.ReasonPhrase}");
                    break;
                }
                else
                {
                    Console.WriteLine("Current VORs cleared.");
                }
            }

            if (fileDate == DateOnly.FromDateTime(DateTime.Now.Date))
            {
                await httpClient.DeleteAsync("/api/vors");
            }

            var failed = false;

            foreach (var i in FileParser.ParseFile(file, fileDate))
            {
                var result = await httpClient.PostAsJsonAsync("/api/vors", i);

                if (!result.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Received Error : {result.StatusCode}");
                    failed = true;
                }
            }

            if (!failed)
            {
                var dirPath = Path.GetDirectoryName(file) ?? Environment.CurrentDirectory;
                Directory.CreateDirectory(Path.Combine(dirPath, "Uploaded"));
                File.Move(file, Path.Combine(dirPath, "Uploaded", Path.GetFileName(file)));
            }
        }
    }
}
