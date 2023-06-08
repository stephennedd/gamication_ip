using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
