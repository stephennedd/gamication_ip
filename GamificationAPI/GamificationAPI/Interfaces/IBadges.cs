using GamificationAPI.Models;
using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface IBadges
    {
        Task<List<Badge>> GetBadgesAsync();
        Task<Badge> GetBadgeAsync(int BadgeId);
        Task<bool> AddBadgeAsync(Badge Badge);
        Task<bool> UpdateBadgeAsync(Badge Badge);
        Task<bool> DeleteBadgeAsync(int BadgeId);
        Task<bool> BadgeExistsAsync(int BadgeId);

    }
}
