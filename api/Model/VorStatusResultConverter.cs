// -----------------------------------------------------------------------
// <copyright file="VorStatusResultConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Model;

/// <summary>
/// Converts VorStatusResults to and from JSON.
/// </summary>
public class VorStatusResultConverter : JsonConverter<VorStatusResult>
{
    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Not implemented.</exception>
    public override VorStatusResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, VorStatusResult value, JsonSerializerOptions options)
    {
        if (!value.IsVor)
            writer.WriteBooleanValue(false);
        else
        {
            writer.WriteStartObject();

            writer.WritePropertyName("isVor");
            writer.WriteBooleanValue(value.IsVor);

            if (value.DueBack.HasValue)
            {
                writer.WritePropertyName("dueBack");
                writer.WriteStringValue(value.DueBack.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrWhiteSpace(value.Summary))
            {
                writer.WritePropertyName("summary");
                writer.WriteStringValue(value.Summary.Trim());
            }

            writer.WriteEndObject();
        }
    }
}
