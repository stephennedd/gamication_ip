using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class Leaderboard
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<HighScore> HighScores { get; set; } = new List<HighScore> { };

    }
}
