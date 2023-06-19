using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GamificationAPI.Controllers
{
    [Authorize(Roles = "Admin", Policy = "IsVerified")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public RolesController(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        [HttpPost]
        public IActionResult AddRole([FromBody] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Role name cannot be null or empty");
            }

            _dbContext.Roles.AddAsync(new Role { Name = role, Id = 0 });
            _dbContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            return Ok(_dbContext.Roles.ToList());
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteRole(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be greater than zero");
            }

            var role = _dbContext.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound($"Role with id: {id} not found");
            }

            _dbContext.Roles.Remove(role);
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}