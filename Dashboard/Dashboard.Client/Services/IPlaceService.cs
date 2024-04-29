using Dashboard.Client.Model;

namespace Dashboard.Client.Services;

public interface IPlaceService
{
    IAsyncEnumerable<string> GetDistricts(Region region);
    IAsyncEnumerable<string> GetHubs(Region region, string district);
}
