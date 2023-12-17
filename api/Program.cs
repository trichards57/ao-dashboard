// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using API.Support;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(c => c.AddUserSecrets<Program>())
    .ConfigureServices(s =>
    {
        s.AddSingleton(sp =>
        {
            var configuration = sp.GetService<IConfiguration>();
            return new CosmosClient(configuration["CosmosDBConnection"]);
        });
        s.AddTransient<ICosmosLinqQuery, CosmosLinqQueryHelper>();
    })
    .Build();

host.Run();
