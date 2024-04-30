namespace Dashboard.Client.Model;

public class VorStatistics
{
    /// <summary>
    /// Gets the total number of vehicles at a place.
    /// </summary>
    public int TotalVehicles { get; init; }

    /// <summary>
    /// Gets the number of vehicles available at a place.
    /// </summary>
    public int AvailableVehicles { get; init; }

    /// <summary>
    /// Gets the number of vehicles currently showing as VOR at a place.
    /// </summary>
    public int VorVehicles { get; init; }

    /// <summary>
    /// Gets the past availability of vehicles at a place.
    /// </summary>
    public IReadOnlyDictionary<DateOnly, int> PastAvailability { get; init; } = new Dictionary<DateOnly, int>();
}
