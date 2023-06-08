using GamificationToIP.Models;

namespace GamificationAPI.Models
{
    public class GeneratedTest
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int TestId { get; set; }
        public int numberOfCorrectAnswers { get; set; } = 0;
        public User Student { get; set; }
        public Test Test { get; set; }
    }
}
