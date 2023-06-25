
using GamificationAPI.Models;
using GamificationToIP.Controllers;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Interfaces
{
    public interface IUsers
    {
        Task<List<User>> GetUsersAsync();
        Task<List<User>> GetStudentsAsync();
        Task<User> GetUserByIdAsync(string UserId);
        Task AddUserAsync(User User);
        Task UpdateUserAsync(User User);
        Task DeleteUserAsync(string UserId);
        User GetUserById(string id);
        Task<bool> UserExistsAsync(string UserId);
        Task<bool> VerifyUser(string UserId, string code);
        Task<bool> ChangePasswordAsync(string UserId, string newPassword);
        Task<bool> AddBadgeAsync(Badge badge, string userId);
        Task UpdateStudentAsync(int id, [FromBody] UserUpdateDto userDto);

        Task<bool> AddGroupToUserAsync(string userId, string groupName);

    }
}
