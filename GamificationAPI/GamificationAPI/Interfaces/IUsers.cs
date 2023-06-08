
using GamificationToIP.Models;

namespace GamificationAPI.Interfaces
{
    public interface IUsers
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task AddUserAsync(User User);
        Task UpdateUserAsync(User User);
        Task DeleteUserAsync(string id);
        User GetUserById(string id);
        Task<bool> UserExistsAsync(string id);
        Task<bool> VerifyUser(string id, string code);
    }
}
