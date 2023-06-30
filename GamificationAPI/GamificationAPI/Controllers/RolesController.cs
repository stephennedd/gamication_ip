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

        [HttpGet]
        public IActionResult GetRoles()
        {
            try
            {
                return Ok(_dbContext.Roles.ToList());
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}