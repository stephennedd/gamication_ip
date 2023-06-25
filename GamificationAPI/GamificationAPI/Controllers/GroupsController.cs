using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
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
            _dbContext.Groups.AddAsync(new Group { Name = groupName});
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
