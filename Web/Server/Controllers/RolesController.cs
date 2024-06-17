using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.Constants;
using ModelLib.DTOs.Roles;
using Web.Server.Constants;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.ROLES)]
    [ApiController]
    [Authorize(Roles = RoleConstants.ADMIN)]
    public class RolesController : CustomControllerBase
    {
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        public RolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager) : base(userManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.userId.ToString());
            var result = await _userManager.AddToRoleAsync(user, dto.roleName);
            if (!result.Succeeded)
            {
                return Conflict(result);
            }

            return Ok(result);
        }

        [HttpPost("create/{roleName}")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            return Ok(await _roleManager.CreateAsync(new IdentityRole<int>(roleName)));
        }

    }
}
