using GamificationToIP.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GamificationAPI.Models
{
    public class Badge
    {
        [Key]
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
