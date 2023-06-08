
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace GamificationAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class HighScoresController : ControllerBase
    {
        private readonly ILeaderboards _leaderboardService;
        private readonly IHighScores _highScoreService;
        private readonly IUsers _userService;

        public HighScoresController(ILeaderboards leaderboardService, IHighScores highScoreService, IUsers userService)
        {
            _leaderboardService = leaderboardService;
            _highScoreService = highScoreService;
            _userService = userService;
        }


        // POST: api/scoreboard
        // checks if it really is an high score than adds it to leaderboard
        [HttpPost]
        public async Task<IActionResult> AddHighScoreToLeaderboard([FromBody] HighScore highScore, string leaderboardName)
        {
            if (highScore is null || string.IsNullOrEmpty(leaderboardName) || !ModelState.IsValid || await _userService.UserExistsAsync(highScore.User.Id) == false)
            {
                return BadRequest();
            }

            if (await _leaderboardService.CheckIfStudentHasHighScoreInLeadeboard(highScore.User.Id, leaderboardName) == true)
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
        [Authorize(Roles = "Admin, Teacher")]
        [HttpDelete]
        public async Task<IActionResult> DeleteHighScoreById(int highScoreId)
        {
            try
            {
                await _highScoreService.DeleteHighScoreAsync(highScoreId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


    }
    



}
