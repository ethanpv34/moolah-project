using Microsoft.AspNetCore.Mvc;
using MoolahApi.Models.DTOs;
using MoolahApi.Services;
using System.Threading.Tasks;

namespace MoolahApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required");
            }

            var response = await _authService.RegisterAsync(request);
            if (response == null)
            {
                return BadRequest("User already exists");
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(response);
        }
    }
} 