using GamificationAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string GameName { get; set; }
        public ICollection<Subject> Subjects { get; set; }
    }
}
