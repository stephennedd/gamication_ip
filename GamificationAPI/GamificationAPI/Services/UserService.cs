
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

public class UserService : IUsers
{
    private readonly ApplicationDbContext _dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetUserByIdAsync(string UserId)
    {
        return await _dbContext.Users.Include(u => u.Role).Include(u => u.Group).Include(u => u.Badges).FirstOrDefaultAsync(u => u.UserId == UserId);
    }


    public async Task AddUserAsync(User user)
    {

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User User)
    {
        _dbContext.Users.Update(User);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ChangePasswordAsync(string UserId, string newPassword)
    {
        var user = await GetUserByIdAsync(UserId);

        if (user == null)
        {
            return false;
        }
        user.Password = newPassword;
        await UpdateUserAsync(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task DeleteUserAsync(string UserId)
    {
        var User = await GetUserByIdAsync(UserId);

        if (User != null)
        {
            _dbContext.Users.Remove(User);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _dbContext.Users.Include(u => u.Role).Include(u => u.Group).Include(u => u.Badges).ToListAsync();
    }

    public User GetUserById(string UserId)
    {
        return _dbContext.Users.Include(u => u.Role).Include(u => u.Group).Include(u => u.Badges).FirstOrDefault(u => u.UserId == UserId);
    }
    public Task<bool> UserExistsAsync(string UserId)
    {
        return _dbContext.Users.AnyAsync(s => s.UserId == UserId);
    }
    public Task<bool> VerifyUser(string UserId, string code)
    {
        var user = GetUserById(UserId);
        if (user != null)
        {
            if (user.VerificationCode == code)
            {
                user.IsVerified = true;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }
    public async Task<bool> AddBadgeAsync(Badge badge, string userId)
    {
        var user = await GetUserByIdAsync(userId);

        if (user == null)
        {
            return false;
        }
        var newbadge = await _dbContext.Set<Badge>().FirstOrDefaultAsync(u => u.Id == badge.Id);
        user.Badges.Add(newbadge);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
