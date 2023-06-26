
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Admin, Teacher, Student", Policy = "IsVerified")]
[Route("api/[controller]")]
[ApiController]
public class LeaderboardsController : ControllerBase
{
    private readonly ILeaderboards _leaderboardService;
    private readonly IUsers _userService;
    private readonly ApplicationDbContext _context;

    public LeaderboardsController(ILeaderboards leaderboardService, IUsers userService, ApplicationDbContext context)
    {
        _leaderboardService = leaderboardService;
        _userService = userService;
        _context = context;
    }

    // GET: api/scoreboard
    [HttpGet]
    public async Task<IActionResult> GetAllLeaderboard()
    {
        var leaderboard = await _leaderboardService.GetLeaderboardsSimpleAsync();
        if (!leaderboard.Any())
        {
            return NoContent();
        }

        return Ok(leaderboard);
    }
    [HttpGet("{LeaderboardName}")]
    public async Task<IActionResult> GetLeaderboardById(string LeaderboardName, string? group)
    {
        if (string.IsNullOrEmpty(LeaderboardName))
        {
            return BadRequest();
        }

        var leaderboard = await _leaderboardService.GetLeaderboardByNameAsync(LeaderboardName);

        if (leaderboard is null)
        {
            return NotFound();
        }
        if (group != null)
        {
            
            if (group == "mygroup")
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
                var user = await _userService.GetUserByIdAsync(userId);
                if (user is null)
                {
                    return BadRequest($"Could not find user {userId}");
                }
                var thisgroup = user.Group;
                if (thisgroup is null)
                {
                    return BadRequest("User is not part of any group");
                }
                List<HighScore> highScores = new List<HighScore>();
                foreach (var item in leaderboard.HighScores)
                {
                    if (item.User.Group == thisgroup)
                    {
                        highScores.Add(item);

                    }

                }
                leaderboard.HighScores.Clear();
                leaderboard.HighScores = highScores;

                return Ok(leaderboard);
            }
            else
            {
                if(await _context.Groups.AnyAsync(s => s.Name == group) == false)
                {
                    return BadRequest("Group with this name does not exist");
                }
                var thisgroup = await _context.Groups.FirstOrDefaultAsync(s => s.Name == group);
                if (thisgroup is null)
                {
                    return BadRequest("Something went wrong");
                }
                List<HighScore> highScores = new List<HighScore>();
                foreach (var item in leaderboard.HighScores)
                {
                    if (item.User.Group == thisgroup)
                    {
                        highScores.Add(item);
                    }

                }
                leaderboard.HighScores.Clear();
                leaderboard.HighScores = highScores;
                return Ok(leaderboard);
            }
        }

        return Ok(leaderboard);
    }
    [Authorize(Roles = "Admin, Teacher")]
    [HttpPost]
    public async Task<IActionResult> CreateNewLeaderboard(string leaderboardName)
    {
        if (string.IsNullOrWhiteSpace(leaderboardName))
        {
            return BadRequest("Leaderboard name cannot be empty.");
        }

        var existingLeaderboard = await _leaderboardService.GetLeaderboardByNameAsync(leaderboardName);
        if (existingLeaderboard != null)
        {
            return Conflict("A leaderboard with this name already exists.");
        }

        await _leaderboardService.CreateLeaderboardAsync(leaderboardName);

        return Ok();
    }
    [Authorize(Roles = "Admin, Teacher")]
    [HttpDelete("{leaderboardName}")]
    public async Task<IActionResult> DeleteLeaderboard(string leaderboardName)
    {
        if (string.IsNullOrWhiteSpace(leaderboardName))
        {
            return BadRequest("Leaderboard name cannot be empty.");
        }

        var existingLeaderboard = await _leaderboardService.GetLeaderboardByNameAsync(leaderboardName);
        if (existingLeaderboard == null)
        {
            return NotFound("No leaderboard with this name exists.");
        }

        bool x = await _leaderboardService.DeleteLeaderboardAsync(leaderboardName);
        if (x)
        {
            return Ok();
        }

        return BadRequest("Something went wrong.");
        
    }
    [Authorize(Roles = "Admin, Teacher")]
    [HttpPut("{leaderboardName}")]
    public async Task<IActionResult> UpdateLeaderboard(string leaderboardName, string newLeaderboardName)
    {
        if (string.IsNullOrWhiteSpace(leaderboardName))
        {
            return BadRequest("Leaderboard name cannot be empty.");
        }
        var existingLeaderboard = await _leaderboardService.GetLeaderboardByNameAsync(leaderboardName);
        if (existingLeaderboard == null)
        {
            return NotFound("No leaderboard with this name exists.");
        }
        bool x = await _leaderboardService.UpdateLeaderboardAsync(leaderboardName, newLeaderboardName);
        if (x)
        {
            return Ok();
        }
        return BadRequest("Something went wrong.");
    }


}