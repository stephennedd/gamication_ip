using GamificationToIP.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamificationAPI.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string SubjectTitle { get; set; }
        public int WeekNumber { get; set; }

        [ForeignKey("Test")]
        public int TestId { get; set; } 

        public virtual Test Test { get; set; } 
    }
}
