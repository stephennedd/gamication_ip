using GamificationToIP.Models;
using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class Badge
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string imageUrl { get; set; }
        public string Description { get; set; }

        public User User { get; set; }
    }
}
