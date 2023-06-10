
using GamificationToIP.Models;

namespace GamificationAPI.Interfaces
{
    public interface IUsers
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(string UserId);
        Task AddUserAsync(User User);
        Task UpdateUserAsync(User User);
        Task DeleteUserAsync(string UserId);
        User GetUserById(string id);
        Task<bool> UserExistsAsync(string UserId);
        Task<bool> VerifyUser(string UserId, string code);
    }
}
