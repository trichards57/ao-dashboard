using Dashboard.Client.Services;
using Dashboard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Dashboard.Controllers
{
    [Route("api/users")]
    [Authorize(Policy = "CanViewUsers")]
    [ApiController]
    public class UserController(IUserService userService, UserManager<ApplicationUser> userManager, ILogger<RoleController> logger) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IUserService userService = userService;

        [HttpGet]
        public IAsyncEnumerable<UserWithRole> Get() => userService.GetUsersWithRole();

        [HttpGet("{id}")]
        public async Task<ActionResult<UserWithRole>> Get([Required] string id)
        {
            var user = await userService.GetUserWithRole(id);

            if (user == null)
            {
                logger.LogWarning("Role {Id} not found", id);
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanEditUsers")]
        public async Task<ActionResult> Put([Required] string id, [FromBody] UserRoleUpdate role)
        {
            var currentId = userManager.GetUserId(User);

            if (id.Equals(currentId, StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (await userService.SetUserRole(id, role))
            {
                return Ok();
            }

            return NotFound();
        }
    }
}
