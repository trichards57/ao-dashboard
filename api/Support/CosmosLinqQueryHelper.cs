// -----------------------------------------------------------------------
// <copyright file="CosmosLinqQueryHelper.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq;

namespace API.Support;

public interface ICosmosLinqQuery 
{
    FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query);
}

internal class CosmosLinqQueryHelper : ICosmosLinqQuery
{
    public FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query) => query.ToFeedIterator();
}
