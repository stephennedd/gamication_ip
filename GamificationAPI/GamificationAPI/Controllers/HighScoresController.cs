
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

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
        public async Task<IActionResult> AddHighScoreToLeaderboard(int score, string leaderboardName)
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

            if (await _userService.UserExistsAsync(userId) == false)
            {
                return BadRequest();
            }

            if (score == null || string.IsNullOrEmpty(leaderboardName) || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var highScore = new HighScore
            {
                Score = score,
                User = await _userService.GetUserByIdAsync(userId)
            };


            if (await _leaderboardService.CheckIfStudentHasHighScoreInLeadeboard(highScore.User.UserId, leaderboardName) == true)
            {
                if (await _highScoreService.CheckIfItsHighScore(highScore, leaderboardName) == true)
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
                bool success = await _leaderboardService.AddHighScoreAsync(highScore, leaderboardName);
                if (success)
                {
                    return Ok();
                }
                return BadRequest();
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