
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Controllers;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using BCrypt.Net;

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
    public async Task UpdateStudentAsync(int id, [FromBody] UserUpdateDto userDto)
    {
        var user = _dbContext.Users.Find(id);

        // Update the user fields
        user.Name = userDto.Name;
        user.Surname = userDto.Surname;
        user.Username = userDto.Username;
        if (userDto.Password != "")
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        }

        _dbContext.SaveChanges();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _dbContext.Users.Include(u => u.Role).Include(u => u.Group).Include(u => u.Badges).ToListAsync();
    }

    public async Task<List<User>> GetStudentsAsync()
    {
        return await _dbContext.Users
        .Include(u => u.Role)
        .Include(u => u.Group)
        .Include(u => u.Badges)
        .Where(u => u.Role.Name == "Student")
        .ToListAsync();
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
    public async Task<bool> AddGroupToUserAsync(string userId, string groupName)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        var group = await _dbContext.Set<Group>().FirstOrDefaultAsync(u => u.Name == groupName);
        if (group == null)
        {
            return false;
        }
        user.Group = group;
        await _dbContext.SaveChangesAsync();
        return true;

    }



}
