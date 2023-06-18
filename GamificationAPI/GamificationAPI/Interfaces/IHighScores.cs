using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface IHighScores
    {
        Task<List<HighScore>> GetHighScoresAsync();
        Task<HighScore> GetHighScoreByIdAsync(int id);
        Task<List<HighScore>> GetHighScoreByStudentIdAsync(string UserId);
        Task<bool> CheckIfItsHighScore(HighScore newHighScore, string leaderboardName);
        Task AddHighScoreAsync(HighScore highScore);
        Task UpdateHighScoreAsync(HighScore highScore, string leaderboardName);
        Task DeleteHighScoreAsync(int id);
        Task<bool> UpdateMainLeaderboard(string userId);
    }
}
