﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Authorization;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GamificationToIP.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUsers _userService;

        public UsersController(ApplicationDbContext context, IUsers userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Users
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // GET: api/Users/5

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if ( await _userService.UserExistsAsync(id) == false )
            {
                return NotFound();
            }

            var User = await _userService.GetUserByIdAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            return Ok(User);
        }

        // POST: api/Users
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserCredentials userCredentials)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.UserExistsAsync(userCredentials.Id))
                {
                    return BadRequest("User with this ID already exists");
                }

                User newUser = new User { Id = userCredentials.Id, Password = userCredentials.Password };
                if (IsDigitsOnly(userCredentials.Id))
                {
                    newUser.Role = _context.Roles.FirstOrDefault(x => x.Id == 1);
                }
                else
                {
                    newUser.Role = _context.Roles.FirstOrDefault(x => x.Id == 2);
                }

                await _userService.AddUserAsync(newUser);
                return CreatedAtAction("GetUser", new { id = newUser.Id }, newUser);
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
            Console.WriteLine(userId);
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
               
                    if(await _userService.VerifyUser(userId, token))
                    {
                        return Ok();
                    }
                    return BadRequest();
                
                return BadRequest("User with this ID does not exist");
            }
            return BadRequest();
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, User User)
        {
            if (id != User.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _userService.UserExistsAsync(User.Id) == false)
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

        // DELETE: api/Users/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {

            if(await _userService.UserExistsAsync(id) == false)
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