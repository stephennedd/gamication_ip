using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboards _leaderboardService;

    public LeaderboardController(ILeaderboards leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    // GET: api/scoreboard
    [HttpGet]
    public async Task<IActionResult> GetAllLeaderboard()
    {
        var leaderboard = await _leaderboardService.GetLeaderboardsAsync();
        return Ok(leaderboard);
    }
    [HttpGet("{LeaderboardName}")]
    public async Task<IActionResult> GetLeaderboardById(string LeaderboardName)
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

        return Ok(leaderboard);
    }
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