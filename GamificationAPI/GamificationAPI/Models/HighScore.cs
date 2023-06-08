
using GamificationToIP.Models;
using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class HighScore
    {
        [Key]
        public int Id { get; set; }
        public User Student { get; set; }
        public int Score { get; set; }
    }
}
