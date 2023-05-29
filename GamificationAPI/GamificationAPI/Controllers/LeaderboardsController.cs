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
    [HttpPost]
    public async Task<IActionResult> CreateNewLeaderboard(string LeaderboardName)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _leaderboardService.CreateLeaderboardAsync(LeaderboardName);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteLeaderboard(string LeaderboardName)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _leaderboardService.DeleteLeaderboardAsync(LeaderboardName);
        return Ok();
    }

}