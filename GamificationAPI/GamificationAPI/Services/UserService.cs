
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

    public async Task<User> GetUserByIdAsync(string id)
    {
        return await _dbContext.Users.Include(u => u.Role).Include(u => u.Group).FirstOrDefaultAsync(u => u.Id == id);
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

    public async Task DeleteUserAsync(string id)
    {
        var User = await GetUserByIdAsync(id);

        if (User != null)
        {
            _dbContext.Users.Remove(User);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _dbContext.Users.Include(u => u.Role).Include(u => u.Group).ToListAsync();
    }

    public User GetUserById(string id)
    {
        return _dbContext.Users.Include(u => u.Role).Include(u => u.Group).FirstOrDefault(u => u.Id == id);
    }
    public Task<bool> UserExistsAsync(string id)
    {
        return _dbContext.Users.AnyAsync(s => s.Id == id);
    }
    public Task<bool> VerifyUser(string id, string code)
    {
        var user = GetUserById(id);
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