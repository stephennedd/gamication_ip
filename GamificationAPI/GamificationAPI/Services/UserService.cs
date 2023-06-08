
using GamificationAPI.Interfaces;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;

public class UserService : IUsers
{
    private readonly ApplicationDbContext _dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetUserByIdAsync(string id)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(s => s.Id == id);
    }


    public async Task AddUserAsync(User User)
    {
        _dbContext.Users.Add(User);
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
        return await _dbContext.Users.ToListAsync();
    }

    public User GetUserById(string id)
    {
        return _dbContext.Users.FirstOrDefault(s => s.Id == id);
    }
    public Task<bool> UserExistsAsync(string id)
    {
        return _dbContext.Users.AnyAsync(s => s.Id == id);
    }
}