using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApp.Utils.ExternalLoginProviders.Facebook;
using ModelLib.Constants;
using WebApp.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;
using ModelLib.ApiDTOs;
using EntityLib.Entities.Identity;

namespace WebApp.Controllers
{
    [Route(ApiEndpoints.AUTHENTICATION)]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFacebookLoginProvider _facebookLoginProvider;

        public AuthenticationController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IFacebookLoginProvider facebookLoginProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _facebookLoginProvider = facebookLoginProvider;
        }

        [HttpPost(ApiEndpoints.AUTHENTICATION_REGISTER)]
        public async Task<ActionResult<UserInfoDTO>> Register([FromBody] RegisterDTO register)
        {
            if (register.Password != register.RepeatedPassword)
            {
                return BadRequest("passwords does not match");
            }
            if (register.Email != register.RepeatedEmail)
            {
                return BadRequest("emails does not match");
            }
            var user = await _userManager.FindByEmailAsync(register.Email);
            if (user != null)
            {
                return BadRequest("Email already exists!");
            }

            user = new ApplicationUser()
            {
                Email = register.Email,
                UserName = register.FirstName + " " + register.LastName,
                FirstName = register.FirstName,
                LastName = register.LastName
            };

            var registerResult = await _userManager.CreateAsync(user, register.Password);
            if (registerResult.Succeeded)
            {
                return Ok("User registered");
            }
            return BadRequest(registerResult.Errors.Select(e => e.Code + ": " + e.Description));
        }

        /// <summary>
        /// Logs the user in.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {        
        ///       "email": "developer_user@hotmail.com",
        ///       "password": "Test123!"
        ///     }
        /// </remarks>
        /// <param name="LoginDTO"></param>   
        [HttpPost(ApiEndpoints.AUTHENTICATION_LOGIN)]
        public async Task<ActionResult<UserInfoDTO>> Login([FromBody] LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                return Unauthorized();
            }
            
            var signInResult = await _signInManager.PasswordSignInAsync(user, login.Password, true, false);
            
            if (signInResult.Succeeded)
            {
                return Ok();
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.AUTHENTICATION_USER_INFO)]
        public async Task<ActionResult<UserInfoDTO>> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return new UserInfoDTO()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePictureUrl = user.ProfileImageUrl
            };
        }

        [Authorize]
        [HttpGet(ApiEndpoints.AUTHENTICATION_LOG_OUT)]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }


        [HttpPost(ApiEndpoints.AUTHENTICATION_FACEBOOK_LOGIN)]
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
                var newUser = new ApplicationUser()
                {
                    Email = facebookUser.Email,
                    UserName = $"{facebookUser.FirstName} {facebookUser.LastName}",
                    EmailConfirmed = true,
                    FirstName = facebookUser.FirstName,
                    LastName = facebookUser.LastName,
                    ProfileImageUrl = facebookUser.Picture.Data.Url.ToString()

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
