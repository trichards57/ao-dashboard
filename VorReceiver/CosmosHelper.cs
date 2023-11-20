// -----------------------------------------------------------------------
// <copyright file="CosmosHelper.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace VorReceiver;

/// <summary>
/// Helper functions for working with CosmosDb containers.
/// </summary>
internal static class CosmosHelper
{
    /// <summary>
    /// Gets the container that contains VOR data.
    /// </summary>
    /// <param name="cosmosClient">The client used to retrieve the container.</param>
    /// <param name="configuration">The configuration data used to control the cosmos details.</param>
    /// <returns>A CosmosDb container containing the VOR data.</returns>
    public static Container GetVorContainer(this CosmosClient cosmosClient, IConfiguration configuration)
        => cosmosClient.GetContainer(
            configuration.GetValue("CosmosDbDatabase", "vehicle-data"),
            configuration.GetValue("CosmosDbContainer", "vor-data"));
}
