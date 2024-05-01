using System.ComponentModel.DataAnnotations;

namespace Dashboard.Client.Model;

public class UpdateVehicleSettings
{
    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    [Required]
    public string Registration { get; init; } = "";

    /// <summary>
    /// Gets the owning hub.
    /// </summary>
    [Required]
    public string Hub { get; init; } = "";

    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    [Required]
    public string CallSign { get; init; } = "";

    /// <summary>
    /// Gets the owning district.
    /// </summary>
    [Required]
    public string District { get; init; } = "";

    /// <summary>
    /// Gets the owning region.
    /// </summary>
    [EnumDataType(typeof(Region))]
    [Required]
    public Region Region { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    [EnumDataType(typeof(VehicleType))]
    [Required]
    public VehicleType VehicleType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is marked for disposal.
    /// </summary>
    public bool ForDisposal { get; init; }
}
