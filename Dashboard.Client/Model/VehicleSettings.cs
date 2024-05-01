namespace Dashboard.Client.Model;

public class VehicleSettings
{
    /// <summary>
    /// Gets the internal ID for the vehicle.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    public string Registration { get; init; }

    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the owning hub.
    /// </summary>
    public string Hub { get; init; }

    /// <summary>
    /// Gets the owning district.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the owning region.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    public VehicleType VehicleType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is marked for disposal.
    /// </summary>
    public bool ForDisposal { get; init; }
}
