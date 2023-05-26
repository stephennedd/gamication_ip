using BulkyBookWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace GamificationToIP.Models
{
    public class StudentResult
    {
        [Key]
        public int Id { get; set; }
        public int NumberOfHpPoints { get; set; }
        public Student student { get; set; } = null!;
        public Test Test { get; set; } = null!;
    }
}
