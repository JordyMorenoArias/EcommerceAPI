using EcommerceAPI.Models.DTOs.User;
using EcommerceAPI.Services.Auth.Interfaces;
using EcommerceAPI.Services.User.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    /// <summary>
    /// Controller responsible for user authentication and account management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="userService">The user service.</param>
        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates a user and returns a token.
        /// </summary>
        /// <param name="UserLogin">Login credentials.</param>
        /// <returns>JWT and user info if successful.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto UserLogin)
        {
            var response = await _authService.Login(UserLogin);
            return Ok(response);
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="userRegister">User registration info.</param>
        /// <returns>Status message indicating success or failure.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegister)
        {
            var result = await _authService.Register(userRegister);

            if (result)
                return Ok(new { Message = "User registered successfully" });
            else
                return BadRequest(new { Message = "User registration failed" });
        }

        /// <summary>
        /// Confirms the user's email using a token.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="token">Confirmation token.</param>
        /// <returns>Status message.</returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmail(email, token);

            if (result)
                return Ok(new { Message = "Email confirmed successfully" });
            else
                return BadRequest(new { Message = "Email confirmation failed" });
        }

        /// <summary>
        /// Allows an authenticated user to change their password.
        /// </summary>
        /// <param name="userChangePassword">Current and new password info.</param>
        /// <returns>Status message.</returns>
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

        /// <summary>
        /// Sends a password reset email to the user.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <returns>Status message.</returns>
        [HttpPost("send-forgot-password")]
        public async Task<IActionResult> SendForgotPasswordEmail([FromBody] string email)
        {
            var result = await _authService.SendForgotPasswordEmail(email);

            if (result)
                return Ok(new { Message = "Password reset email sent successfully" });
            else
                return BadRequest(new { Message = "Password reset email failed" });
        }

        /// <summary>
        /// Verifies the reset code sent to the user's email.
        /// </summary>
        /// <param name="verifyResetCode">Reset code verification data.</param>
        /// <returns>Status message.</returns>
        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] UserVerifyResetCodeDto verifyResetCode)
        {
            var result = await _authService.VerifyResetCode(verifyResetCode);

            if (result)
                return Ok(new { Message = "Reset code verified successfully" });
            else
                return BadRequest(new { Message = "Reset code verification failed" });
        }

        /// <summary>
        /// Resets the user's password using the provided reset token/code.
        /// </summary>
        /// <param name="userResetPassword">Password reset info.</param>
        /// <returns>Status message.</returns>
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