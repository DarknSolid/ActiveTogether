using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Web.Server.Utils.ExternalLoginProviders.Facebook;
using ModelLib.Constants;
using Microsoft.AspNetCore.Authorization;
using ModelLib.ApiDTOs;
using EntityLib.Entities.Identity;
using ModelLib.DTOs.Authentication;
using ModelLib.Repositories;
using RazorLib;
using System.Web;

namespace Web.Server.Controllers
{
    [Route(ApiEndpoints.AUTHENTICATION)]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUsersRepository _usersRepository;
        private readonly IFacebookLoginProvider _facebookLoginProvider;
        private readonly EmailClient _emailClient;

        public AuthenticationController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IFacebookLoginProvider facebookLoginProvider, IUsersRepository usersRepository, EmailClient emailClient)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _facebookLoginProvider = facebookLoginProvider;
            _usersRepository = usersRepository;
            _emailClient = emailClient;
        }

        [HttpPost(ApiEndpoints.AUTHENTICATION_REGISTER)]
        public async Task<ActionResult<RegisterResultDTO>> Register([FromBody] RegisterDTO register)
        {
            RegisterResultDTO result = new RegisterResultDTO();
            result.ErrorMessages = new List<string>();

            if (register.Password != register.RepeatedPassword)
            {
                result.ErrorMessages.Add("Passwords do not match");
            }
            if (register.Email != register.RepeatedEmail)
            {
                result.ErrorMessages.Add("Emails do not match");
            }
            if (!register.ConfirmAtLeastThirteen)
            {
                result.ErrorMessages.Add("You must be at least 13 years old to create an account");
            }
            var user = await _userManager.FindByEmailAsync(register.Email);
            if (user != null)
            {
                result.ErrorMessages.Add("Email is already registered!");
            }

            if (result.ErrorMessages.Any())
            {
                return Ok(result);
            }

            user = new ApplicationUser()
            {
                Email = register.Email,
                UserName = register.FirstName + " " + register.LastName,
                FirstName = register.FirstName,
                LastName = register.LastName,
                AtLeastThirteenYearsOld = register.ConfirmAtLeastThirteen
            };

            var registerResult = await _userManager.CreateAsync(user, register.Password);
            if (registerResult.Succeeded)
            {
                if (register.ProfilePicture is not null && register.ProfilePicture.ContainsFile())
                {
                    await _usersRepository.UploadProfilePictureAsync(register.ProfilePicture, user.Id);
                }

                result.Success = true;
                result.UserInfo = new UserDetailedDTO
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Id = user.Id,
                    ProfilePictureUrl = user.ProfileImageUrl
                };

                // send confirm email
                result.MustConfirmEmail = true;
                await SendEmailConfirmation(user);
            }
            else
            {
                registerResult.Errors.ToList().ForEach(e => result.ErrorMessages.Add(e.Description));
            }

            return Ok(result);
        }

        private async Task SendEmailConfirmation(ApplicationUser? user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = HttpUtility.UrlEncode(token);
            var redirect = GetBaseUrl() + RoutingConstants.NEW_USER_WELCOME;
            var url = GetBaseUrl() + $"{RoutingConstants.CONFIRM_EMAIL}?email={user.Email}&token={token}&redirect={redirect}";

            _emailClient.SendEmailConfirmationMail(user.Email, user.FirstName, url, GetBaseUrl());
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
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginDTO login)
        {
            var result = new LoginResult();

            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null)
            {
                return Unauthorized();
            }
            else if (!user.EmailConfirmed)
            {
                result.MustConfirmEmail = true;
                await SendEmailConfirmation(user);
                return Ok(result);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, login.Password, true, false);

            if (signInResult.Succeeded)
            {
                result.Success = true;
                result.UserDetailedInfo = await _usersRepository.GetUserAsync(user.Id, user.Id);
                return Ok(result);
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpGet(ApiEndpoints.AUTHENTICATION_USER_INFO)]
        public async Task<ActionResult<UserDetailedDTO>> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return await _usersRepository.GetUserAsync(user.Id, user.Id);
        }

        [Authorize]
        [HttpGet(ApiEndpoints.AUTHENTICATION_LOG_OUT)]
        public async Task<ActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost(ApiEndpoints.AUTHENTICATION_FACEBOOK_LOGIN)]
        public async Task<ActionResult<SignInWithThirdPartyResult>> FacebookLogin([FromBody] FacebookLoginDTO loginDto)
        {
            var isValid = await _facebookLoginProvider.ValidateAccessToken(loginDto.AccessToken);
            var facebookUser = await _facebookLoginProvider.GetUserInfo(loginDto.AccessToken);
            if (!isValid || facebookUser is null) return Ok(new SignInWithThirdPartyResult
            {
                Errors = new List<string>() { "Facebook authentication failed" }
            });

            var user = await _userManager.FindByEmailAsync(facebookUser.Email);

            UserLoginInfo loginInfo = null;
            if (facebookUser is not null)
            {
                loginInfo = new UserLoginInfo(
                        _facebookLoginProvider.Provider,
                        facebookUser.Id,
                        _facebookLoginProvider.Provider);
            }

            if (user != null)
            {
                var hasExistingFacebookProvider = await _userManager.FindByLoginAsync(_facebookLoginProvider.Provider, facebookUser.Id) != null;
                if (!hasExistingFacebookProvider)
                {
                    await _userManager.AddLoginAsync(user, loginInfo);
                }
                await _signInManager.SignInAsync(user, isPersistent: true);
                return new SignInWithThirdPartyResult
                {
                    Success = true,
                    DidRegisterNewUser = false,
                    UserInfo = await _usersRepository.GetUserAsync(user.Id)
                };

            }
            else
            {
                var result = new SignInWithThirdPartyResult();
                var newUser = new ApplicationUser()
                {
                    Email = facebookUser.Email,
                    UserName = $"{facebookUser.FirstName} {facebookUser.LastName}",
                    EmailConfirmed = true,
                    FirstName = facebookUser.FirstName,
                    LastName = facebookUser.LastName,
                    ProfileImageUrl = facebookUser.Picture.Data.Url.ToString(),
                    AtLeastThirteenYearsOld = true,

                };
                var createdResponse = await _userManager.CreateAsync(newUser);

                if (!createdResponse.Succeeded)
                {
                    result.Errors = createdResponse.Errors.Select(it => it.Description).ToList();
                    return Ok(result);
                }

                result.Success = true;
                result.DidRegisterNewUser = true;
                result.UserInfo = await _usersRepository.GetUserAsync(newUser.Id, newUser.Id);

                await _userManager.AddLoginAsync(newUser, loginInfo);
                await _signInManager.SignInAsync(newUser, isPersistent: true);

                return Ok(result);
            }
        }

        [HttpPost(ApiEndpoints.AUTHENTICATION_CONFIRM_EMAIL)]
        public async Task<ActionResult<bool>> ConfirmEmail([FromBody] ConfirmEmailDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return false;
            }
            if (user.EmailConfirmed)
            {
                return true;
            }
            var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
            }
            return result.Succeeded;
        }

        [Authorize]
        [HttpPut(ApiEndpoints.AUTHENTICATION_CHANGE_PASSWORD)]
        public async Task<ActionResult<ChangePasswordResultDTO>> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var resultDto = new ChangePasswordResultDTO();
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                resultDto.Success = true;
                return Ok(resultDto);
            }
            else
            {
                resultDto.Errors = result.Errors.Select(e => e.Description).ToList();
                return Ok(resultDto);
            }
        }

        [Authorize]
        [HttpPost(ApiEndpoints.AUTHENTICATION_REQUEST_CHANGE_EMAIL)]
        public async Task<IActionResult> RequestChangeEmail([FromBody] RequestChangeEmailDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var existingEmail = await _userManager.FindByEmailAsync(dto.NewEmail);
            if (existingEmail is not null)
            {
                return Ok(new RequestChangeEmailResultDTO
                {
                    Success = false,
                    Errors = new List<string>() { "Email already exists." }
                });
            }
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.NewEmail);
            token = HttpUtility.UrlEncode(token);
            var redirect = GetBaseUrl() + RoutingConstants.USER_SETTINGS;
            var url = GetBaseUrl() + $"{RoutingConstants.CONFIRM_EMAIL}?email={dto.NewEmail}&token={token}&redirect={redirect}&isChangeEmail=true&currentEmail={user.Email}";
            _emailClient.SendChangeEmailConfirmationMail(recipient: dto.NewEmail, newEmail: dto.NewEmail, tokenUrl: url, userFirstName: user.FirstName, webAppBaseUri: GetBaseUrl());
            return Ok(new RequestChangeEmailResultDTO { Success = true });

        }

        [HttpPost(ApiEndpoints.AUTHENTICATION_CHANGE_EMAIL)]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.CurrentEmail);
            if (user is null)
            {
                return BadRequest();
            }
            var result = await _userManager.ChangeEmailAsync(user, dto.NewEmail, dto.Token);
            if (result.Succeeded)
            {
                return Ok("Email changed to " + dto.NewEmail);
            }
            return BadRequest();
        }

        [HttpGet(ApiEndpoints.AUTHENTICATION_FORGOT_PASSWORD + "{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = HttpUtility.UrlEncode(token);
                var url = GetBaseUrl() + RoutingConstants.RESET_PASSWORD + $"?email={email}&token={token}";
                _emailClient.SendPasswordResetEmail(user.Email, user.FirstName, url, GetBaseUrl());
            }
            return Ok();
        }

        [HttpPost(ApiEndpoints.AUTHENTICATION_RESET_PASSWORD)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email); 
            if (user is null)
            {
                return NotFound();
            }
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(error: result.Errors.ToList().Select(e => e.Description));
        }


        private string GetBaseUrl()
        {
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.ToString();
            return scheme + "://" + host + "/";
        }

    }
}
