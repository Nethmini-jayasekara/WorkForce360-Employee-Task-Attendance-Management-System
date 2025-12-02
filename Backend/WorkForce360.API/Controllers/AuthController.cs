using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkForce360.API.Data;
using WorkForce360.API.DTOs;
using WorkForce360.API.Models;
using WorkForce360.API.Services;

namespace WorkForce360.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                Role = "Employee",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    DateOfJoining = user.DateOfJoining,
                    IsActive = user.IsActive
                }
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Account is deactivated. Please contact administrator." });
            }

            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    DateOfJoining = user.DateOfJoining,
                    IsActive = user.IsActive
                }
            });
        }
    }
}
