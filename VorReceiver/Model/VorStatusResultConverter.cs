// -----------------------------------------------------------------------
// <copyright file="VorStatusResultConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace VorReceiver.Model;

/// <summary>
/// Converts VorStatusResults to and from JSON.
/// </summary>
public class VorStatusResultConverter : JsonConverter<VorStatusResult>
{
    /// <inheritdoc/>
    public override VorStatusResult ReadJson(JsonReader reader, Type objectType, VorStatusResult existingValue, bool hasExistingValue, JsonSerializer serializer) => throw new NotImplementedException();

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, VorStatusResult value, JsonSerializer serializer)
    {
        if (!value.IsVor)
        {
            writer.WriteValue(false);
        }
        else
        {
            writer.WriteStartObject();

            writer.WritePropertyName("isVor");
            writer.WriteValue(value.IsVor);

            if (value.DueBack.HasValue)
            {
                writer.WritePropertyName("dueBack");
                writer.WriteValue(value.DueBack.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrWhiteSpace(value.Summary))
            {
                writer.WritePropertyName("summary");
                writer.WriteValue(value.Summary.Trim());
            }

            writer.WriteEndObject();
        }
    }
}
