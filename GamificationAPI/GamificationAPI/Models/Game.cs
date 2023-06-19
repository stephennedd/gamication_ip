using System.ComponentModel.DataAnnotations;

namespace GamificationToIP.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string GameName { get; set; }
        public ICollection<Subject> Subjects { get; set; }
    }
}
