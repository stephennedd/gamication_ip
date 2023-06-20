
using Microsoft.AspNetCore.Mvc;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Authorization;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using System.Security.Claims;

using NuGet.Common;
using Microsoft.VisualStudio.Web.CodeGeneration;
using GamificationAPI.Services;
using Newtonsoft.Json;
using BCrypt.Net;


namespace GamificationToIP.Controllers
{
  //  [Authorize(Roles = "Admin, Teacher, Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUsers _userService;
        private readonly IEmails _emailService;
        private readonly IBadges _badgeService;    


       

        public UsersController(ApplicationDbContext context, IUsers userService, IEmails emailService, IBadges badgeService)

        {
            _context = context;
            _emailService = emailService;
            _userService = userService;
            _badgeService = badgeService;
            _emailService = emailService;

        }

        // GET: api/Users
       // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var json = JsonConvert.SerializeObject(users, Formatting.None, jsonSettings);
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }
     
        [HttpGet("role/students")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var students = await _userService.GetStudentsAsync();
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var json = JsonConvert.SerializeObject(students, Formatting.None, jsonSettings);
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // GET: api/Users/5
        [Authorize(Policy = "IsVerified")]
        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUser(string UserId)
        {
            if (await _userService.UserExistsAsync(UserId) == false)
            {
                return NotFound();
            }

            var User = await _userService.GetUserByIdAsync(UserId);
            if (User == null)
            {
                return NotFound();
            }

            return Ok(User);
        }

        // POST: api/Users
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateStudent(UserRegister userCredentials)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.UserExistsAsync(userCredentials.UserId))
                {
                    return BadRequest("User with this ID already exists");
                }


                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCredentials.Password);
                User newUser = new User { UserId = userCredentials.UserId, Password = userCredentials.hashedPassword, Username = userCredentials.Name, Surname = userCredentials.Surname };

                // Hash the password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCredentials.Password);


                if (IsDigitsOnly(userCredentials.UserId))
                {
                    newUser.Role = _context.Roles.FirstOrDefault(x => x.Id == 1);
                }
                else
                {
                    return BadRequest("Only other teacher can create teacher account");
                }
                
                await _userService.AddUserAsync(newUser);
                //TODO: Send email with verification token to UserId + @domain
                EmailDto Email = new EmailDto { To = "t6666349@gmail.com", Subject = "Verify your account", Body = $"Your verification token is: {newUser.VerificationCode}" };
                _emailService.SendEmail(Email);
                return CreatedAtAction("GetUser", new { UserId = newUser.UserId }, newUser);
            }
            return BadRequest();
        }
        // POST: api/Users
        [Authorize(Roles = "Admin, Teacher", Policy = "IsVerified")]
        [HttpPost]
        [Route("Admin")]
        public async Task<IActionResult> CreateTeacher(TeacherRegister teacherRegister, bool admin)
        {
            if (ModelState.IsValid)
            {
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                {
                    return BadRequest("Authorization header is missing.");
                }
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(userRole))
                {
                    return BadRequest("Invalid token.");
                }
                if(userRole == "Student")
                {
                    return BadRequest("Student cannot access this method");
                }
                if (await _userService.UserExistsAsync(teacherRegister.UserId))
                {
                    return BadRequest("User with this ID already exists");
                }

                User newUser = new User { UserId = teacherRegister.UserId, Password = CodeGenerator.RandomString(8), Name = teacherRegister.Name, Surname = teacherRegister.Surname };
                if (IsDigitsOnly(teacherRegister.UserId))
                {
                    BadRequest("Student account cant be created by teacher");
                }
                else
                {
                    if (admin && userRole == "Admin")
                    {
                        newUser.Role = _context.Roles.FirstOrDefault(x => x.Id == 3);
                    }
                    else
                    {
                        newUser.Role = _context.Roles.FirstOrDefault(x => x.Id == 2);
                    }
                }
                newUser.IsVerified = true;
                await _userService.AddUserAsync(newUser);
                //TODO: Send email with password to UserId + @domain
                EmailDto Email = new EmailDto { To = "t6666349@gmail.com", Subject = "Your Gamification Password", Body = $"Your new Password is: {newUser.VerificationCode} You can change it any time" };
                _emailService.SendEmail(Email);
                return CreatedAtAction("GetUser", new { UserId = newUser.UserId }, newUser);
            }
            return BadRequest();
        }

        [HttpPost("{token}")]
        public async Task<IActionResult> VerifyUser(string token)
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return BadRequest("Authorization header is missing.");
            }
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token.");
            }

            if (await _userService.UserExistsAsync(userId) == false)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {

                if (await _userService.VerifyUser(userId, token))
                {
                    return Ok();
                }

                return BadRequest("User with this ID does not exist");
            }
            return BadRequest();
        }

        // PUT: api/Users/5
      //  [Authorize(Policy = "IsVerified")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string UserId, User User)
        {
            if (UserId != User.UserId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _userService.UserExistsAsync(User.UserId) == false)
                    {
                        return NotFound("User with this ID does not exist");
                    }
                    await _userService.UpdateUserAsync(User);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                return NoContent();
            }
            return Ok(User);
        }
        [Authorize(Policy = "IsVerified")]
        [HttpPatch]
        public async Task<IActionResult> ChangePassword(string newPassword)
        {
            if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                return BadRequest("Authorization header is missing.");
            }
            var userId = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token.");
            }
            if (await _userService.UserExistsAsync(userId) == false)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                bool success = await _userService.ChangePasswordAsync(userId, newPassword);

                if (success)
                {
                    return Ok();
                }

                return BadRequest("User with this ID does not exist");
            }
            return BadRequest();
        }
        [Authorize(Policy = "IsVerified")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> AddBadgeToUser (string id, int badgeId)
        {
            if (await _userService.UserExistsAsync(id) == false)
            {
                return NotFound("User with this ID does not exist");
            }
            if(await _badgeService.BadgeExistsAsync(badgeId) == false)
            {
                return BadRequest("Badge with this ID does not exist");
            }
            var badge = await _badgeService.GetBadgeAsync(badgeId);
            if (badge == null)
            {
                return BadRequest();
            }
            bool success = await _userService.AddBadgeAsync(badge, id);
            if (success)
            {
                return Ok();
            }
            return BadRequest();

        }

        // PUT: api/users/ban/{id}
        [HttpPut("ban/{id}")]
        public IActionResult BanUser(int id, [FromBody] bool isBanned)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsBanned = isBanned;
            _context.SaveChanges();

            return Ok();
        }

        // DELETE: api/Users/5
        //   [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {

            if (await _userService.UserExistsAsync(id) == false)
            {
                return NotFound("User with this ID does not exist");
            }

            var User = await _userService.GetUserByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);

            return Ok();
        }
        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }



    }
}