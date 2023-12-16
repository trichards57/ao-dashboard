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

/// <summary>
/// Interface that represents a FeedIterator helper.
/// </summary>
public interface ICosmosLinqQuery
{
    /// <summary>
    /// Gets a feed iterator for the provided query.
    /// </summary>
    /// <typeparam name="T">The query item type.</typeparam>
    /// <param name="query">The query.</param>
    /// <returns>The feed iterator for the query.</returns>
    FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query);
}

/// <summary>
/// The default FeedIterator helper for Cosmos LINQ queries.
/// </summary>
internal class CosmosLinqQueryHelper : ICosmosLinqQuery
{
    /// <inheritdoc/>
    public FeedIterator<T> GetFeedIterator<T>(IQueryable<T> query) => query.ToFeedIterator();
}
