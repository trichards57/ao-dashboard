
using System.Net.Http.Json;

namespace Dashboard.Client.Services;

internal class UserService(HttpClient httpClient, ILogger<UserService> logger) : IUserService
{
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<UserService> logger = logger;

    public async Task<UserWithRole?> GetUserWithRole(string id)
    {
        var response = await httpClient.GetAsync($"api/users/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserWithRole>();
        }

        logger.LogError("Getting Permissions {Id} Failed : {StatusCode}", id, response.StatusCode);

        return null;
    }

    public async IAsyncEnumerable<UserWithRole> GetUsersWithRole()
    {
        var response = await httpClient.GetAsync($"api/users");

        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<UserWithRole>>();

            if (users != null)
            {
                foreach (var user in users)
                {
                    yield return user;
                }
            }
        }
    }

    public async Task<bool> SetUserRole(string id, UserRoleUpdate permissions)
    {
        var response = await httpClient.PutAsJsonAsync($"api/users/{id}", permissions);

        return response.IsSuccessStatusCode;
    }
}
