using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface IHighScores
    {
        Task<List<HighScore>> GetHighScoresAsync();
        Task<HighScore> GetHighScoreByIdAsync(int id);
        Task<HighScore> GetHighScoreByStudentIdAsync(int studentId);
        Task AddHighScoreAsync(HighScore highScore);
        Task UpdateHighScoreAsync(HighScore highScore);
        Task DeleteHighScoreAsync(int id);
    }
}
