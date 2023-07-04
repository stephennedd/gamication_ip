using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student", Policy = "IsVerified")]
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
        public IActionResult AddGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return BadRequest("No group name provided");
            }
            var group = _dbContext.Groups.FirstOrDefault(g => g.Name == groupName);
            if(group != null) 
            { 
                return BadRequest("Group already exists");
            }
            _dbContext.Groups.AddAsync(new Group { Name = groupName});
            _dbContext.SaveChanges();
            return Ok();
        }
        [Authorize(Roles = "Admin, Teacher")]
        [HttpPatch]
        [Route("{name}")]
        public IActionResult EditGroup(string name, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                return BadRequest("No group name provided");
            }
            var group = _dbContext.Groups.FirstOrDefault(g => g.Name == name);
            if (group == null)
            {
                return NotFound();
            }
            group.Name = newName;
            _dbContext.SaveChanges();
            return Ok();
        }

        [AllowAnonymous]
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
            List<User> users = _dbContext.Users.ToList();
            foreach (var user in users)
            {
                if (user.Group != null)
                {
                    if (user.Group.Id == id)
                    {
                        user.Group = null;
                    }
                }
            }
            _dbContext.SaveChanges();
            _dbContext.Groups.Remove(group);
            _dbContext.SaveChanges();
            return Ok();
        }


    }
}
