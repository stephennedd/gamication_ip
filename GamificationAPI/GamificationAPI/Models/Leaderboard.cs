using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class Leaderboard
    {
        [Key]
        public string Name { get; set; }
        public ICollection<HighScore> HighScores { get; set; } = null!;
    }
}
