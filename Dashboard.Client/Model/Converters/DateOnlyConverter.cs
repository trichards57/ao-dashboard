// -----------------------------------------------------------------------
// <copyright file="DateOnlyConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Dashboard.Client.Model.Converters;

/// <summary>
/// Converter for <see cref="DateOnly"/> to and from gRPC.
/// </summary>
public static class DateOnlyConverter
{
    /// <summary>
    /// Converts a <see cref="DateOnly"/> to a gRPC <see cref="Grpc.DateOnly"/>.
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <returns>The converted date.</returns>
    public static Grpc.DateOnly? ToGrpc(DateOnly? date)
    {
        if (date == null)
        {
            return null;
        }

        return new Grpc.DateOnly
        {
            Year = (uint)date.Value.Year,
            Month = (uint)date.Value.Month,
            Day = (uint)date.Value.Day,
        };
    }

    /// <summary>
    /// Converts a gRPC <see cref="Grpc.DateOnly"/> to a <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <returns>The converted date.</returns>
    public static Grpc.DateOnly ToGrpc(DateOnly date) => new()
    {
        Year = (uint)date.Year,
        Month = (uint)date.Month,
        Day = (uint)date.Day,
    };

    /// <summary>
    /// Converts a gRPC <see cref="Grpc.DateOnly"/> to a <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="date">The date to convert.</param>
    /// <returns>The converted date.</returns>
    public static DateOnly ToData(Grpc.DateOnly date) => new((int)date.Year, (int)date.Month, (int)date.Day);
}
