using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;

namespace GamificationAPI.Services
{
    public class BadgeService : IBadges
    {
        private readonly ApplicationDbContext _dbContext;

        public BadgeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddBadgeAsync(Badge Badge)
        {
            try
            {
                await _dbContext.Badges.AddAsync(Badge);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Task<bool> BadgeExistsAsync(int BadgeId)
        {
            return _dbContext.Badges.AnyAsync(s => s.Id == BadgeId);
        }

        public async Task<bool> DeleteBadgeAsync(int BadgeId)
        {
            try
            {
                var badge = await GetBadgeAsync(BadgeId);

                if (badge != null)
                {
                    _dbContext.Badges.Remove(badge);
                    await _dbContext.SaveChangesAsync();
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Task<Badge> GetBadgeAsync(int BadgeId)
        {      
                var badge = _dbContext.Badges.FirstOrDefaultAsync(u => u.Id == BadgeId);
            if (badge != null)
            {

                return badge;
            }
            throw new InvalidOperationException("Cannot find badge with this id");
        }

        public Task<List<Badge>> GetBadgesAsync()
        {
            return _dbContext.Badges.ToListAsync();
        }

        public async Task<bool> UpdateBadgeAsync(Badge badge)
        {
            try
            {
                _dbContext.Badges.Update(badge);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
