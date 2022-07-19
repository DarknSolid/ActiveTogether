using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.Repositories;
using static ModelLib.Repositories.RepositoryEnums;

namespace WebApp.Controllers
{
    public class CustomControllerBase : ControllerBase
    {

        protected readonly UserManager<ApplicationUser> _userManager;

        public CustomControllerBase(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<int> GetAuthorizedUserId()
        {
            return (await _userManager.GetUserAsync(User)).Id;
        }

        protected ActionResult ResolveRepositoryResponse(ResponseType response, object returnedValue = null)
        {
            switch (response)
            {
                case ResponseType.Created: return Ok(returnedValue);
                case ResponseType.Updated: return Ok(returnedValue);
                case ResponseType.Conflict: return Conflict();
                case ResponseType.NotFound: return NotFound();
                case ResponseType.Deleted: return Ok();
                default: throw new NotImplementedException($"Resolve is not implemented for repository response type: {response}");
            }
            
        }
    }
}
