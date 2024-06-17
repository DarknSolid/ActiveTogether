using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLib.ApiDTOs;
using ModelLib.Constants;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.FEEDBACK)]
    [ApiController]
    public class FeedbackController : CustomControllerBase
    {
        private readonly EmailClient _emailClient;

        public FeedbackController(EmailClient emailClient, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _emailClient = emailClient;
        }

        [HttpPost]
        public async Task<IActionResult> PostFeedback([FromBody] FeedbackCreateDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            _emailClient.SendFeedback(user, dto);
            return Ok();
        }
    }
}
