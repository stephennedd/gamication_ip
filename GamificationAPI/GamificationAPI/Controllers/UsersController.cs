﻿
using Microsoft.AspNetCore.Mvc;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using System.Security.Claims;

using NuGet.Common;
using Microsoft.VisualStudio.Web.CodeGeneration;
using GamificationAPI.Services;
using Newtonsoft.Json;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GamificationAPI.Controllers
{
  
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
        [Authorize(Roles = "Admin")]
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
                User newUser = new User { UserId = userCredentials.UserId, Password = hashedPassword, Username = userCredentials.Name, Surname = userCredentials.Surname };

                if (IsDigitsOnly(userCredentials.UserId))
                {
                    newUser.Role = _context.Roles.FirstOrDefault(x => x.Id == 1);
                }
                else
                {
                    return BadRequest("Only other teacher can create teacher account");
                }
                
                await _userService.AddUserAsync(newUser);
                //Send email with password to UserId + @domain
                string studentMail = userCredentials.UserId + "@student.saxion.nl";
                EmailDto Email = new EmailDto { To = studentMail, Subject = "Verify your account", Body = $"Your verification token is: {newUser.VerificationCode}" };
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
                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) || string.IsNullOrWhiteSpace(authorizationHeader))
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
                string generatedCode = CodeGenerator.RandomString(8);
                User newUser = new User { UserId = teacherRegister.UserId, Password = BCrypt.Net.BCrypt.HashPassword(generatedCode), Name = teacherRegister.Name, Surname = teacherRegister.Surname };
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
                //Send email with password to UserId + @domain
                string teacherMail = teacherRegister.UserId + "@saxion.nl";
                EmailDto Email = new EmailDto { To = teacherMail, Subject = "Your Gamification Password", Body = $"Your new Password is: {generatedCode} You can change it any time" };
                _emailService.SendEmail(Email);
                return CreatedAtAction("GetUser", new { UserId = newUser.UserId }, newUser);
            }
            return BadRequest();
        }

        [HttpPost("{token}")]
        public async Task<IActionResult> VerifyUser(string token)
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
        [Authorize(Policy = "IsVerified")]
        [HttpPut("{userId}")]
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
                    return Ok(User);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                
            }
            return BadRequest();
        }
        [Authorize(Policy = "IsVerified")]
        [HttpPatch]
        public async Task<IActionResult> ChangePassword(string newPassword)
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
        [HttpPatch("{userId}")]
        public async Task<IActionResult> AddBadgeToUser (string userId, int badgeId)
        {
            if (await _userService.UserExistsAsync(userId) == false)
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
            bool success = await _userService.AddBadgeAsync(badge, userId);
            if (success)
            {
                return Ok();
            }
            return BadRequest();

        }

        [HttpPatch("Group")]
        [Authorize(Roles = "Student, Admin, Teacher")]
        public async Task<IActionResult> AddGroupToUser(string groupName)
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
            if (await _userService.UserExistsAsync(userId) == false)
            {
                return BadRequest("user with this id does not exist");
            }
            if(groupName.IsNullOrEmpty())
            {
                return BadRequest("Group name must contain 'Group' word");
            }
            bool success = await _userService.AddGroupToUserAsync(userId, groupName);
            if (success)
            {
                return Ok();
            }
            return BadRequest("Group with this name does not exist");

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
        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {

            if (await _userService.UserExistsAsync(userId) == false)
            {
                return NotFound("User with this ID does not exist");
            }

            var User = await _userService.GetUserByIdAsync(userId);
            if (User == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(userId);

            return Ok();
        }

        [Authorize(Roles = "Admin, Teacher", Policy = "IsVerified")]
        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UserUpdateDto userDto)
        {
            await _userService.UpdateStudentAsync(id,userDto);

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

    public class UserUpdateDto
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
    }

}