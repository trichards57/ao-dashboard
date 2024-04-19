namespace Dashboard.Client.Services;

public interface IRoleService
{
    IAsyncEnumerable<RolePermissions> GetRoles();
    Task<RolePermissions?> GetRolePermissions(string id);
    Task<bool> SetRolePermissions(string id, RolePermissionsUpdate permissions);
}

public enum ReadWrite
{
    Read,
    Write,
    Deny
}

public sealed class RolePermissions
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ReadWrite VehicleConfiguration { get; set; }
    public ReadWrite VorData { get; set; }
    public ReadWrite Permissions { get; set; }
}

public sealed class RolePermissionsUpdate
{
    public ReadWrite VehicleConfiguration { get; set; }
    public ReadWrite VorData { get; set; }
    public ReadWrite Permissions { get; set; }
}

