
using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface ILeaderboards
    {
        Task<List<Leaderboard>> GetLeaderboardsAsync();
        Task<Leaderboard> GetLeaderboardByNameAsync(string name);
        Task<bool> AddHighScoreAsync(HighScore highScore, string leaderboardName);
        Task<bool> UpdateLeaderboardAsync(string oldName, string newName);
        Task<bool> DeleteLeaderboardAsync(string name);
        Task<bool> CreateLeaderboardAsync(string name);
        Task<bool> CheckIfStudentHasHighScoreInLeadeboard(string studentId, string name);
        Task<List<Leaderboard>> GetLeaderboardsSimpleAsync();
    }
}