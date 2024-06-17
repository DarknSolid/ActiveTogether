using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.Repositories;
using static ModelLib.Repositories.RepositoryEnums;

namespace Web.Server.Controllers
{
    public class CustomControllerBase : ControllerBase
    {

        protected readonly UserManager<ApplicationUser> _userManager;

        public CustomControllerBase(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<int?> GetAuthorizedUserIdAsync()
        {
            return (await _userManager.GetUserAsync(User))?.Id ?? null;
        }

        protected ActionResult ResolveRepositoryResponse(ResponseType response, object? returnedValue = null)
        {
            switch (response)
            {
                case ResponseType.Ok:
                case ResponseType.Created:
                case ResponseType.Updated: return Ok(returnedValue);
                case ResponseType.Deleted: return Ok();
                case ResponseType.Conflict: return Conflict();
                case ResponseType.NotFound: return NotFound();
                default: throw new NotImplementedException($"Resolve is not implemented for repository response type: {response}");
            }
            
        }
    }
}
