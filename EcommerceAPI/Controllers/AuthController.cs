using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Services.Interfaces;
using EcommerceAPI.Services.Security.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto UserLogin)
        {
            var response = await _authService.Login(UserLogin);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
        {
            var result = await _authService.Register(userRegister);

            if (result)
                return Ok(new { Message = "User registered successfully" });
            else
                return BadRequest(new { Message = "User registration failed" });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmail(email, token);

            if (result)
                return Ok(new { Message = "Email confirmed successfully" });
            else
                return BadRequest(new { Message = "Email confirmation failed" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto userChangePassword)
        {
            var userAuth = _userService.GetAuthenticatedUser(HttpContext);

            var result = await _authService.ChangePassword(userAuth.Id, userChangePassword);

            if (result)
                return Ok(new { Message = "Password changed successfully" });
            else
                return BadRequest(new { Message = "Password change failed" });
        }

        [HttpPost("send-forgot-password")]
        public async Task<IActionResult> SendForgotPasswordEmail([FromBody] string email)
        {
            var result = await _authService.SendForgotPasswordEmail(email);

            if (result)
                return Ok(new { Message = "Password reset email sent successfully" });
            else
                return BadRequest(new { Message = "Password reset email failed" });
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] UserVerifyResetCodeDto verifyResetCode)
        {
            var result = await _authService.VerifyResetCode(verifyResetCode);

            if (result)
                return Ok(new { Message = "Reset code verified successfully" });
            else
                return BadRequest(new { Message = "Reset code verification failed" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto userResetPassword)
        {
            var result = await _authService.ResetPassword(userResetPassword);

            if (result)
                return Ok(new { Message = "Password reset successfully" });
            else
                return BadRequest(new { Message = "Password reset failed" });
        }
    }
}
