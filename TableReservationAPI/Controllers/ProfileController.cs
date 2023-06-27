using ApplicationService.Models.UserModels;
using ApplicationService.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TableReservationAPI.CustomMiddleware;

namespace TableReservationAPI.Controllers
{
    [Route("api/profile")]
    [GoogleAuthorized]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAccountService _loginService;
        public ProfileController(IUserService userService, IAccountService loginService)
        {
            _userService = userService;
            _loginService = loginService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfileAsync() { 
            string authHeader = Request.Headers.Authorization;
            try
            {
                var user = await _loginService.ValidateLoginAsync(authHeader);
                var result = await _userService.GetProfile(user.Email);
                return Ok(result);
            }
            catch (InvalidJwtException)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized, "Cannot load payload from token!"));
            }
            catch (Exception)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized, "No token"));
            }
        }
        [HttpPost("update-phone")]
        public async Task<IActionResult> UpdatePhoneNumberAsync(UpdatePhoneModel phoneNumber)
        {
            string authHeader = Request.Headers.Authorization;
            try
            {
                var user = await _loginService.ValidateLoginAsync(authHeader);
                await _userService.ChangePhoneNumber(phoneNumber, user);
                return Ok(StatusCode(StatusCodes.Status204NoContent, "Updated!"));
            }
            catch (KeyNotFoundException)
            {
                return Ok(StatusCode(StatusCodes.Status401Unauthorized, "Cannot load user data!"));
            }
        }
    }
}
