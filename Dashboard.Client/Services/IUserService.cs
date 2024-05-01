namespace Dashboard.Client.Services;

public interface IUserService
{
    IAsyncEnumerable<UserWithRole> GetUsersWithRole();
    Task<UserWithRole?> GetUserWithRole(string id);
    Task<bool> SetUserRole(string id, UserRoleUpdate role);
}

public sealed class UserWithRole
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? RoleId { get; set; }
    public string? Role { get; set; }
}

public sealed class UserRoleUpdate
{
    public string RoleId { get; set; }
}

