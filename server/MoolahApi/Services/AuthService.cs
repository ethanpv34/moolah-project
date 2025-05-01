using Microsoft.EntityFrameworkCore;
using MoolahApi.Data;
using MoolahApi.Models;
using MoolahApi.Models.DTOs;
using System.Threading.Tasks;
using BCrypt.Net;

namespace MoolahApi.Services
{
    public class AuthService
    {
        private readonly TodoDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(TodoDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return null;
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Token = token
            };
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Id = user.Id,
                Username = user.Username,
                Token = token
            };
        }
    }
} 