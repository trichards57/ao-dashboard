using Dashboard.Client.Model;

namespace Dashboard.Client.Services;

public interface IVorService
{
    /// <summary>
    /// Gets the statistics for the VORs in a place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>
    /// The VOR statistics for the place.
    /// </returns>
    Task<VorStatistics> GetVorStatisticsAsync(Place place);
}
