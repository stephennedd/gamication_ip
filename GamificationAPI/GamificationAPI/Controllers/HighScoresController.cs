using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HighScoresController : ControllerBase
    {
        private readonly ILeaderboards _leaderboardService;
        private readonly IHighScores _highScoreService;
        private readonly IStudents _studentService;

        public HighScoresController(ILeaderboards leaderboardService, IHighScores highScoreService, IStudents studentService)
        {
            _leaderboardService = leaderboardService;
            _highScoreService = highScoreService;
            _studentService = studentService;
        }


        // POST: api/scoreboard
        // checks if its is really an high score than adds it to leaderboard
        [HttpPost]
        public async Task<IActionResult> AddHighScoreToLeaderboard([FromBody] HighScore highScore, string leaderboardName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await _leaderboardService.CheckIfStudentHasHighScoreInLeadeboard(highScore.Student, leaderboardName) == true)
            {
                if(await _highScoreService.CheckIfItsHighScore(highScore, leaderboardName) == true)
                {
                    await _highScoreService.UpdateHighScoreAsync(highScore, leaderboardName);
                    return Ok();
                }
                else
                {
                    return BadRequest("This is not High Score");
                }
            }
            else
            {
                await _leaderboardService.AddHighScoreAsync(highScore, leaderboardName);
                return Ok();
            }
      
        }

        
    }
    



}
