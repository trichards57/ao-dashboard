using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VorReceiver
{
    internal static class CosmosHelper
    {
        public static Container GetVorContainer(this CosmosClient cosmosClient, IConfiguration configuration) 
            => cosmosClient.GetContainer(
                configuration.GetValue("CosmosDbDatabase", "vehicle-data"), 
                configuration.GetValue("CosmosDbContainer", "vor-data"));
    }
}
