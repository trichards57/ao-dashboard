using System.Net.Http.Json;

namespace Dashboard.Client.Services;

internal class RoleService : IRoleService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<RoleService> logger;

    public RoleService(HttpClient httpClient, ILogger<RoleService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<RolePermissions?> GetRolePermissions(string id)
    {
        var response = await httpClient.GetAsync($"api/roles/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<RolePermissions>();
        }

        logger.LogError("Getting Permissions {Id} Failed : {StatusCode}", id, response.StatusCode);

        return null;
    }

    public async Task<bool> SetRolePermissions(string id, RolePermissionsUpdate permissions)
    {
        var response = await httpClient.PutAsJsonAsync($"api/roles/{id}", permissions);

        return response.IsSuccessStatusCode;
    }
}
