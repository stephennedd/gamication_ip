using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student", Policy = "IsVerified")]
    [Route("api/[controller]")]
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IBadges _badgeService;

        public BadgesController(ApplicationDbContext context, IBadges badgeService)
        {
            _dbContext = context;
            _badgeService = badgeService;
        }

        [Authorize(Roles = "Admin, Teacher")]
        [HttpPost]
        public async Task<IActionResult> AddBadge(Badge badge)
        {
            if (string.IsNullOrWhiteSpace(badge.Name))
            {
                return BadRequest();
            }
            bool success = await _badgeService.AddBadgeAsync(badge);
            if (success)
            {
                return Ok();
            }
            return BadRequest();
        }


        [HttpPatch]
        public async Task<IActionResult> UpdateBadge([FromBody] Badge badge)
        {
            
            if(await _badgeService.BadgeExistsAsync(badge.Id)== false)
            {
                return NotFound("Badge with this id does not exist");
            }
            bool success = await _badgeService.UpdateBadgeAsync(badge);
            if (success) { 
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetBadges()
        {
            return Ok( await _badgeService.GetBadgesAsync());
        }

        [Authorize(Roles = "Admin, Teacher")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBadge(int id)
        {
            if(await _badgeService.BadgeExistsAsync(id)== false)
            {
                return NotFound("Badge with this id does not exist");
            }
            bool success = await _badgeService.DeleteBadgeAsync(id);
            if(success)
            {
                return Ok();
            }
            return BadRequest();
        }


    }

}
