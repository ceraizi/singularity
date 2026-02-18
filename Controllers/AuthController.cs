using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Singularity.Models;
using Singularity.Data;
using Singularity.DTOs;

namespace Singularity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config){
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto registerDto){
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username)){
            return BadRequest("Username already taken.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        
        var user = new User { 
            Username = registerDto.Username, 
            PasswordHash = passwordHash 
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new {message = "User created successfully!"});
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto loginDto){
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash)){
            return Unauthorized("Invalid username or password.");
        }

        var token = CreateToken(user);
        return Ok(new {token});
    }

    private string CreateToken(User user){
        var claims = new List<Claim>{
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}