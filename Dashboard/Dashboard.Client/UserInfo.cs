
namespace Dashboard.Client
{
    // Add properties to this class and update the server and client AuthenticationStateProviders
    // to expose more information about the authenticated user to the client.
    public class UserInfo
    {
        public required string RealName { get; set; }
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string AmrUsed { get; set; }
        public required DateTimeOffset? LastAuthenticated { get; set; }
        public Dictionary<string, string> OtherClaims { get; set; }
    }
}
