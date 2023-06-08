
using GamificationAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace GamificationToIP.Models
{
    public class StudentResult
    {
        [Key]
        public int Id { get; set; }
        public int numberOfGivenCorrectAnswers { get; set; }
        public User student { get; set; } = null!;
        public GeneratedTest Test { get; set; } = null!;
    }
}
