using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamificationAPI.Models
{
    public class Leaderboard
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<HighScore> HighScores { get; set; } = new List<HighScore> { };

        [ForeignKey("Subject")]
        public int? SubjectId { get; set; }
        public virtual Subject? Subject { get; set; }
    }
}
