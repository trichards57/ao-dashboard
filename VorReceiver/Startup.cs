// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using VorReceiver;

[assembly: FunctionsStartup(typeof(Startup))]

namespace VorReceiver;

/// <summary>
/// Class to manage the function startup.
/// </summary>
public class Startup : FunctionsStartup
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddJsonFile("appsettings.json", true)
        .AddEnvironmentVariables()
        .Build();

    /// <inheritdoc/>
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddSingleton(s =>
        {
            var connectionString = Configuration["CosmosDBConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Please specify a valid CosmosDBConnection in the appSettings.json file or your Azure Functions Settings.");
            }

            return new CosmosClientBuilder(connectionString)
                .Build();
        });
    }
}
