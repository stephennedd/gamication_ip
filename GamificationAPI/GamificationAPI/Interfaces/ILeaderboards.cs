using BulkyBookWeb.Models;
using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface ILeaderboards
    {
        Task<List<Leaderboard>> GetLeaderboardsAsync();
        Task<Leaderboard> GetLeaderboardByNameAsync(string name);
        Task AddHighScoreAsync(HighScore highScore, string leaderboardName);
        Task UpdateLeaderboardAsync(Leaderboard leaderboard);
        Task<bool> DeleteLeaderboardAsync(string name);
        Task<bool> CreateLeaderboardAsync(string name);
        Task UpdateLeaderboardAsync(string name, string newName, string newDescription, string newImageURL);
        Task<bool> CheckIfStudentHasHighScoreInLeadeboard(Student student, string name);
        
    }
}
