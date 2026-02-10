using Microsoft.AspNetCore.Mvc;

namespace SerilogWebApiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation(
                "Login attempt for user {Username}",
                request.Username
            );

            if (request.Username != "admin" || request.Password != "123")
            {
                _logger.LogWarning(
                    "Failed login for user {Username}",
                    request.Username
                );

                return Unauthorized("Login gagal");
            }

            _logger.LogInformation(
                "User {Username} login berhasil",
                request.Username
            );

            return Ok("Login sukses");
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
