using BulkyBookWeb.Models;
using GamificationToIP.Models;

namespace GamificationAPI.Models
{
    public class StudentQuestion
    {
        public int Id { get; set; }
        public int GeneratedTestId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public Question Question { get; set; }
        public Answer Answer { get; set; }
        public GeneratedTest GeneratedTest { get; set; }
    }
}
