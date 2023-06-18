
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Admin, Teacher, Student")]
[Route("api/[controller]")]
[ApiController]
public class LeaderboardsController : ControllerBase
{
    private readonly ILeaderboards _leaderboardService;
    private readonly IUsers _userService;

    public LeaderboardsController(ILeaderboards leaderboardService, IUsers userService)
    {
        _leaderboardService = leaderboardService;
        _userService = userService;
    }

    // GET: api/scoreboard
    [HttpGet]
    public async Task<IActionResult> GetAllLeaderboard()
    {
        var leaderboard = await _leaderboardService.GetLeaderboardsAsync();
        if (!leaderboard.Any())
        {
            return NoContent();
        }

        return Ok(leaderboard);
    }
    [HttpGet("{LeaderboardName}")]
    public async Task<IActionResult> GetLeaderboardById(string LeaderboardName, bool? mygroup)
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
        if(mygroup == true)
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
            if(user is null)
            {
                return BadRequest($"Could not find user {userId}");
            }
            var group = user.Group;
            if (group is null)
            {
                return BadRequest("User is not part of any group");
            }
            List<HighScore> highScores = new List<HighScore>();
            foreach (var item in leaderboard.HighScores) 
            {
                if(item.User.Group == group)
                {
                    highScores.Add(item);
                    
                }
                
            }
            leaderboard.HighScores.Clear();
            leaderboard.HighScores = highScores;
            Console.WriteLine(leaderboard.HighScores.Count);
            return Ok(leaderboard);
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


}