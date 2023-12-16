using System.Text.Json.Serialization;

namespace Shared;

public readonly record struct VehicleSettingsDetail
{
    /// <summary>
    /// Gets or sets the registration of the vehicle.
    /// </summary>
    [JsonPropertyName("reg")]
    public string Registration { get; init; }

    /// <summary>
    /// Gets or sets the radio call sign for the vehicle.
    /// </summary>
    [JsonPropertyName("callSign")]
    public string CallSign { get; init; }

    /// <summary>
    /// Gets or sets the owning district.
    /// </summary>
    [JsonPropertyName("district")]
    public string District { get; init; }

    /// <summary>
    /// Gets or sets the owning region.
    /// </summary>
    [JsonPropertyName("region")]
    public Region Region { get; init; }

    /// <summary>
    /// Gets or sets the vehicle type.
    /// </summary>
    [JsonPropertyName("type")]
    public VehicleType Type { get; init; }
}
