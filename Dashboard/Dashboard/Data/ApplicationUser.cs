using Microsoft.AspNetCore.Identity;

namespace Dashboard.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string RealName { get; set; } = "";
    }

}
