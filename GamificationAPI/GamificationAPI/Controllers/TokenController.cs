using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GamificationAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsers _userService;
        private readonly ApplicationDbContext _dbContext;

        public TokensController(IConfiguration configuration, IUsers userService, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _userService = userService;
            _dbContext = dbContext;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody] UserCredentials userCredentials)
        {
            if (userCredentials == null)
            {
                return BadRequest("User credentials are required.");
            }

            if (string.IsNullOrEmpty(userCredentials.UserId))
            {
                return BadRequest("UserID is required.");
            }

            if (string.IsNullOrEmpty(userCredentials.Password))
            {
                return BadRequest("Password is required.");
            }

            User user =  _userService.GetUserById(userCredentials.UserId);
            if(user == null || user.Role == null || !BCrypt.Net.BCrypt.Verify(userCredentials.Password, user.Password))
            {
                return BadRequest("Incorrect user data.");
            }
            
            

            List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserId),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("Id", user.Id.ToString()),
            new Claim("IsVerified", user.IsVerified.ToString())
        };
            Console.WriteLine(user.IsVerified.ToString());
           
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(45),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = jwt });
        }
        [Authorize(Roles = "Admin, Teacher, Student")]
        [HttpGet]      
        public async Task<IActionResult> RefreshTokenAsync()
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) || string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return BadRequest("Authorization header is missing.");
            }
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token.");
            }
            var user = await _userService.GetUserByIdAsync(userId);
            if (user is null)
            {
                return BadRequest($"Could not find user {userId}");
            }           

            List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserId),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("Id", user.Id.ToString()),
            new Claim("IsVerified", user.IsVerified.ToString())
        };
            Console.WriteLine(user.IsVerified.ToString());

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(45),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new {token = jwt });
        }

    }
}
