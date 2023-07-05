﻿
using GamificationAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class HighScore
    {
        [Key]
        public int Id { get; set; }
        public User User { get; set; }
        public Leaderboard Leaderboard { get; set; }
        public int Score { get; set; }
    }
}
