using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
    }
}
