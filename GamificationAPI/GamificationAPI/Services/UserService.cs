
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
}