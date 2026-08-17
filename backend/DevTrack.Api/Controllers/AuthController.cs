using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevTrack.Application;
using DevTrack.Domain;
using DevTrack.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace DevTrack.Api.Controllers;
public class AuthController(DevTrackDbContext db, IConfiguration config) : ApiControllerBase
{
    private readonly PasswordHasher<User> hasher = new();
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest r) { var email = r.Email.Trim().ToLowerInvariant(); if (await db.Users.AnyAsync(x => x.Email == email)) return Conflict(new ProblemDetails { Title = "Bu e-posta adresiyle kayıtlı bir hesap zaten var." }); var user = new User { Email = email, PasswordHash = "", DisplayName = r.DisplayName.Trim() }; user.PasswordHash = hasher.HashPassword(user, r.Password); db.Add(user); await db.SaveChangesAsync(); return Ok(Create(user)); }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest r) { var user = await db.Users.SingleOrDefaultAsync(x => x.Email == r.Email.Trim().ToLowerInvariant()); if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, r.Password) == PasswordVerificationResult.Failed) return Unauthorized(new ProblemDetails { Title = "E-posta adresi veya şifre hatalı." }); return Ok(Create(user)); }
    private AuthResponse Create(User user) { var key = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured."); var jwt = new JwtSecurityToken(config["Jwt:Issuer"] ?? "DevTrack.Api", config["Jwt:Audience"] ?? "DevTrack.Web", [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email)], expires: DateTime.UtcNow.AddHours(8), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)); return new(new JwtSecurityTokenHandler().WriteToken(jwt), user.DisplayName, user.Email); }
}
