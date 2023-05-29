using System.ComponentModel.DataAnnotations;

namespace GamificationToIP.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public string Identifier { get; set; } = null!;
        public string AnswerText { get; set; } = null!;
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
