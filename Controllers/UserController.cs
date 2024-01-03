using Microsoft.AspNetCore.Mvc;
using MusicBoxServer.Dtos;
using MusicBoxServer.Services;
using MusicBoxServer.Utils;

namespace MusicBoxServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService userService;
        private readonly ApiResponseController response = new();

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            userService = new UserService(configuration);
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("User attempting to log in", loginRequest.Username);

            int userID = userService.AuthenticateUser(loginRequest.Username, loginRequest.Password);

            if (userID <= 0)
            {
                _logger.LogWarning("Login failed for user: {Username}", loginRequest.Username);
                return response.Unauthorized();
            }

            _logger.LogInformation("User logged in successfully: {Username}", loginRequest.Username);
            // 生成令牌或者执行其他登录成功后的操作
            var token = JWT.GenerateJwtToken(userID);

            return response.Success(token, "User login successfully");
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            _logger.LogInformation("New user registration attempt", registerRequest.Username);

            var result = userService.RegisterUser(registerRequest);

            if (!result)
            {
                _logger.LogWarning("Registration failed for user: {Username}", registerRequest.Username);
                return BadRequest("Registration failed");
            }

            _logger.LogInformation("User registered successfully: {Username}", registerRequest.Username);
            return response.Success("", message: "User registered successfully");
        }

        [HttpGet("userinfo/{userID}")]
        public IActionResult GetUserInfo(int userID)
        {
            _logger.LogInformation("Fetching info for user: {Username}", userID);

            var userInfo = userService.GetUserInfo(userID);

            if (userInfo == null)
            {
                return response.NotFound("User not found");
            }

            return response.Success(userInfo);
        }
    }
}
