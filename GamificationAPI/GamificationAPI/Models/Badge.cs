using GamificationToIP.Models;

namespace GamificationAPI.Models
{
    public class Badge
    {
        int Id { get; set; }
        string Name { get; set; }
        string imageUrl { get; set; }
        string Description { get; set; }

        public User User { get; set; }
    }
}
