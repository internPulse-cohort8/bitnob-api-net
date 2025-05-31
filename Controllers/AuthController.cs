using InternPulse4.Core.Application.Interfaces.Repositories;
using InternPulse4.Core.Application.Interfaces.Services;
using InternPulse4.Core.Application.Services;
using InternPulse4.Core.Domain.Entities;
using InternPulse4.Infrastructure.Context;
using InternPulse4.Jwt;
using InternPulse4.Models;
using InternPulse4.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using static InternPulse4.Models.UserModel.UserRequest;

namespace InternPulse4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly PasswordHasherService _passwordHasherService;
        private readonly InternPulseContext _internPulseContext;
        private readonly IEmailService _emailService;
        private readonly SecuritySettings _securitySettings;

        public AuthController(IUserService userService, IConfiguration config, IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator, PasswordHasherService passwordHasherService, InternPulseContext internPulseContext, IEmailService emailService, IOptions<SecuritySettings> securitySettings)
        {
            _userService = userService;
            _config = config;
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasherService=passwordHasherService;
            _internPulseContext = internPulseContext;
            _emailService= emailService;
            _securitySettings = securitySettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRequest request)
        {
            var user = await _userService.CreateUser(request);

            if (user.IsSuccessful == true)
            {
                return Ok(new { user.Value, user.Message });
            }
            else
            {
                return StatusCode(400, user.Message);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            var user = await _userRepository.GetAsync(model.Email);
            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            var newAccessToken = _jwtTokenGenerator.GenerateToken(user.Id.ToString(), user.Email, user.Role);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("login")]
        public async Task<BaseResponse<LoginResponseModel>> Login(LoginRequestModel model)
        {
            var user = await _userRepository.GetAsync(model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Message = "Invalid credentials",
                    IsSuccessful = false
                };
            }

            var accessToken = _jwtTokenGenerator.GenerateToken(user.Id.ToString(), user.Email, user.Role, user.RememberMe);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(); 

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return new BaseResponse<LoginResponseModel>
            {
                Message = "Login successful",
                IsSuccessful = true,

                Value = new LoginResponseModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    RoleName = user.Role.ToString(),
                    FullName = $"{user.FirstName} {user.LastName}",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                }
            };
        }


        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmail request)
        {
            var response = await _userService.ConfirmEmailAsync(request);
            if (!response.IsSuccessful)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetAsync(model.Email);

            if (user == null)
            {
                return BadRequest("Invalid credentials or user not found.");
            }
            if (!_passwordHasherService.VerifyPassword(model.OldPassword, user.Password))
            {
                return BadRequest("The old password you entered is incorrect.");
            }

            string newHashedPassword = _passwordHasherService.HashPassword(model.NewPassword);

            user.Password = newHashedPassword;
            user.DateModified = DateTime.UtcNow; 

            try
            {
                await  _userRepository.UpdateAsync(user);
            }
            catch (DbUpdateException)
            {
                
                return StatusCode(500, "An error occurred while updating the password.");
            }
            return Ok(new { Message = "Password changed successfully." });
        }


        [HttpPost("forgot-password")]
        [AllowAnonymous] 
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetAsync(request.Email);
            if (user == null)
            {
                return Ok(new { Message = "Invalid credentials or user not found." });
            }

           
            byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
            string plainTextToken = Convert.ToBase64String(tokenBytes)
                                        .Replace('+', '-') 
                                        .Replace('/', '_')
                                        .Replace('=', '\0'); 
            string hashedToken = _passwordHasherService.HashPassword(plainTextToken);

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = hashedToken,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_securitySettings.PasswordResetTokenExpirationMinutes),
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };

            await _internPulseContext.PasswordResetTokens.AddAsync(passwordResetToken);
            await _internPulseContext.SaveChangesAsync();

          
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, plainTextToken, user.Id.ToString());

            return Ok(new { Message = "Check your Email for Confirmation" });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous] 
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetAsync(request.Email);

            if (user == null)
            {
                return BadRequest("Invalid request or user not found.");
            }
            var tokenRecord = await _internPulseContext.PasswordResetTokens
                                           .Where(prt => prt.UserId == user.Id &&
                                                         !prt.IsUsed &&
                                                         prt.ExpiryDate > DateTime.UtcNow)
                                           .OrderByDescending(prt => prt.CreatedDate)
                                           .FirstOrDefaultAsync();

            if (tokenRecord == null)
            {
                return BadRequest("Invalid or expired password reset token. Please request a new one.");
            }
            if (!_passwordHasherService.VerifyPassword(request.Token, tokenRecord.TokenHash))
            {
                return BadRequest("Invalid or expired password reset token. Please request a new one.");
            }
            string newHashedPassword = _passwordHasherService.HashPassword(request.NewPassword);
            user.Password = newHashedPassword;
            user.DateModified = DateTime.UtcNow;
            tokenRecord.IsUsed = true;
            tokenRecord.CreatedDate = DateTime.UtcNow; 

     
            var otherTokens = await _internPulseContext.PasswordResetTokens
                                           .Where(prt => prt.UserId == user.Id &&
                                                         prt.Id != tokenRecord.Id &&
                                                         !prt.IsUsed)
                                           .ToListAsync();
            foreach (var token in otherTokens)
            {
                token.IsUsed = true;
                
            }

            try
            {
                await _internPulseContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while resetting the password.");
            }
            return Ok(new { Message = "Password has been reset successfully." });
        }
    }
}
