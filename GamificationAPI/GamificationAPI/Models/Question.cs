using System.ComponentModel.DataAnnotations;

namespace GamificationToIP.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string QuestionText { get; set; } = null!;
        public string CorrectAnswer { get; set; } = null!;
        public string SelectedAnswer { get; set; } = null!;
        public int TestId { get; set; }
        public Test Test { get; set; } = null!;
        public List<Answer> Answers { get; set; } = null!;
    }
}
