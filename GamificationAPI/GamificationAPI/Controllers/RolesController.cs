using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Controllers
{
    [Authorize(Roles = "Admin")]
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
            _dbContext.Roles.AddAsync(new Role { Name = role, Id = 0 });
            _dbContext.SaveChanges();
            return Ok();
        }

        
        [HttpGet]
        public IActionResult GetRoles()
        {
            return Ok(_dbContext.Roles);
        }

       
        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteRole(int id)
        {
            var role = _dbContext.Roles.FirstOrDefault(r => r.Id == id);
            _dbContext.Roles.Remove(role);
            _dbContext.SaveChanges();
            return Ok();
        }


    }
}
