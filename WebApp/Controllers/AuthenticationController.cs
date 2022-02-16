using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApp.Utils.ExternalLoginProviders.Facebook;
using ModelLib.Constants;
using WebApp.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Route(ApiEndpoints.AUTHENTICATION)]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFacebookLoginProvider _facebookLoginProvider;

        public AuthenticationController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IFacebookLoginProvider facebookLoginProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _facebookLoginProvider = facebookLoginProvider;
        }

        [Authorize]
        [HttpGet(ApiEndpoints.AUTHENTICATION_USER_INFO)]
        public async Task<ActionResult<UserInfoDTO>> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return new UserInfoDTO()
            {
                Email = user.Email,
                UserName = user.UserName
            };
        }

        [Authorize]
        [HttpGet(ApiEndpoints.AUTHENTICATION_LOG_OUT)]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }


        [HttpPost(ApiEndpoints.AUTHENTICATION_FACEBOOK)]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginDTO loginDto)
        {
            var isValid = await  _facebookLoginProvider.ValidateAccessToken(loginDto.AccessToken);
            if (!isValid) return Unauthorized();

            var facebookUser = await _facebookLoginProvider.GetUserInfo(loginDto.AccessToken);
            var user = await _userManager.FindByEmailAsync(facebookUser.Email);

            var loginInfo = new UserLoginInfo(
                    _facebookLoginProvider.Provider,
                    facebookUser.Id,
                    _facebookLoginProvider.Provider);
            
            if (user != null)
            {
                var hasExistingFacebookProvider = await _userManager.FindByLoginAsync(_facebookLoginProvider.Provider, facebookUser.Id) != null;
                if (!hasExistingFacebookProvider)
                {
                    await _userManager.AddLoginAsync(user, loginInfo);
                }
                await _signInManager.SignInAsync(user, isPersistent: true);
            }
            else
            {
                var newUser = new IdentityUser()
                {
                    Email = facebookUser.Email,
                    UserName = $"{facebookUser.FirstName} {facebookUser.LastName}",
                    EmailConfirmed = true
                };
                var createdResponse = await _userManager.CreateAsync(newUser);

                if (!createdResponse.Succeeded)
                {
                    return BadRequest(createdResponse.Errors.Select(it =>  $"{it.Code}: {it.Description}"));
                }

                

                await _userManager.AddLoginAsync(newUser, loginInfo);

                await _signInManager.SignInAsync(newUser, isPersistent: true);
                return Ok();
            }
            
            return Ok();
        }
        
    }
}
