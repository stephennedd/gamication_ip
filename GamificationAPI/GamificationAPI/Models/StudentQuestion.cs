using BulkyBookWeb.Models;
using GamificationToIP.Models;

namespace GamificationAPI.Models
{
    public class StudentQuestion
    {
        public int Id { get; set; }
        public int GeneratedTestId { get; set; }
        public int QuestionId { get; set; }
        public bool IsAnswered { get; set; }
        public Question Question { get; set; }
        public GeneratedTest GeneratedTest { get; set; }
    }
}
