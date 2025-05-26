using InternPulse4.Core.Application.Interfaces.Repositories;
using InternPulse4.Core.Application.Interfaces.Services;
using InternPulse4.Models.UserModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        public AuthController(IUserService userService, IConfiguration config, IUserRepository userRepository)
        {
            _userService = userService;
            _config = config;
            _userRepository = userRepository;
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



    }
}
