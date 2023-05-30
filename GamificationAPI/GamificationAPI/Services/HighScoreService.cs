using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.EntityFrameworkCore;

public class HighScoreService : IHighScores
{
    private readonly ApplicationDbContext _dbContext;

    public HighScoreService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HighScore> GetHighScoreByIdAsync(int id)
    {
        return await _dbContext.Set<HighScore>()
            .Include(h => h.Student)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
    public async Task<bool> CheckIfItsHighScore(HighScore newHighScore, string leaderboardName)
    {
        var leaderboard = await _dbContext.Set<Leaderboard>()
            .Include(l => l.HighScores)
            .FirstOrDefaultAsync(l => l.Name == leaderboardName);
        if (leaderboard != null)
        {
            HighScore? highScoreInDB = _dbContext.HighScores.FirstOrDefault(x => x.Student.Id == newHighScore.Student.Id);
            
            if (highScoreInDB != null)
            {
                if(highScoreInDB.Score >= newHighScore.Score)
                {
                    return false;
                }
            }


        }
        return true;

    }

    public async Task<List<HighScore>> GetHighScoreByStudentIdAsync(int studentId)
    {
        return await _dbContext.Set<HighScore>()
            .Include(h => h.Student)
            .Where(h => h.Student.Id == studentId)
            .ToListAsync();
    }

    public async Task AddHighScoreAsync(HighScore highScore)
    {
        _dbContext.Set<HighScore>().Add(highScore);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateHighScoreAsync(HighScore highScore)
    {
        _dbContext.Set<HighScore>().Update(highScore);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteHighScoreAsync(int id)
    {
        var highScore = await GetHighScoreByIdAsync(id);

        if (highScore != null)
        {
            _dbContext.Set<HighScore>().Remove(highScore);
            await _dbContext.SaveChangesAsync();
        }
    }

    public Task<List<HighScore>> GetHighScoresAsync()
    {
        throw new NotImplementedException();
    }

  
}