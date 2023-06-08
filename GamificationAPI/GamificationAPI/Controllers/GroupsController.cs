﻿using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public GroupsController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        [Authorize(Roles = "Admin, Teacher")]
        [HttpPost]
        public IActionResult AddGroup([FromBody] string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return BadRequest();
            }
            _dbContext.Groups.AddAsync(new Group { Name = groupName, Id = 0 });
            _dbContext.SaveChanges();
            return Ok();
        }

       
        [HttpGet]
        public IActionResult GetGroups()
        {
            return Ok(_dbContext.Groups.ToList());
        }

        [Authorize(Roles = "Admin, Teacher")]
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteGroup(int id)
        {
            var group = _dbContext.Groups.FirstOrDefault(g => g.Id == id);
            if (group == null)
            {
                return NotFound();
            }
            _dbContext.Groups.Remove(group);
            _dbContext.SaveChanges();
            return Ok();
        }


    }
}