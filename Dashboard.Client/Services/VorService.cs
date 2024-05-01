using Dashboard.Client.Model;
using System.Net.Http.Json;

namespace Dashboard.Client.Services;

internal class VorService(HttpClient httpClient, ILogger<VorService> logger) : IVorService
{
    public async Task<VorStatistics> GetVorStatisticsAsync(Place place)
    {
        var response = await httpClient.GetAsync($"api/vor/statistics{place.CreateQuery()}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<VorStatistics>() ?? new();
        }

        return new();
    }

    public async IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Place place)
    {
        var response = await httpClient.GetAsync($"api/vor{place.CreateQuery()}");

        if (response.IsSuccessStatusCode)
        {
            var statuses = await response.Content.ReadFromJsonAsync<List<VorStatus>>();

            if (statuses != null)
            {
                foreach (var status in statuses)
                {
                    yield return status;
                }
            }
        }
    }
}
