﻿using BulkyBookWeb.Models;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HighScoresController : ControllerBase
    {
        private readonly LeaderboardService _leaderboardService;
        private readonly HighScoreService _highScoreService;
        private readonly StudentService _studentService;

        public HighScoresController(LeaderboardService leaderboardService, HighScoreService highScoreService, StudentService studentService)
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
                    //update high score in db
                }
                else
                {
                    return BadRequest("This is not High Score");
                }
            }

            return Ok();
        }

        
    }
    



}
