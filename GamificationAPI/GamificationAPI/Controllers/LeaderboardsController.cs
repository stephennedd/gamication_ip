using BulkyBookWeb.Models;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ScoreboardController : ControllerBase
{
    private readonly LeaderboardService _leaderboardService;
    private readonly HighScoreService _highScoreService;
    private readonly StudentService _studentService;

    public ScoreboardController(LeaderboardService leaderboardService, HighScoreService highScoreService, StudentService studentService)
    {
        _leaderboardService = leaderboardService;
        _highScoreService = highScoreService;
        _studentService = studentService;
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