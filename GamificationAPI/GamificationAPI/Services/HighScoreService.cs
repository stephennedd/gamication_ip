
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
            .Include(h => h.User)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
    public async Task<bool> CheckIfItsHighScore(HighScore newHighScore, string leaderboardName)
{
    var leaderboard = await _dbContext.Set<Leaderboard>()
        .Include(l => l.HighScores)
        .FirstOrDefaultAsync(l => l.Name == leaderboardName);
    if (leaderboard != null)
    {
        HighScore? highScoreInDB = _dbContext.HighScores.FirstOrDefault(x => x.User.UserId == newHighScore.User.UserId);
        
        if (highScoreInDB != null)
        {
            if(highScoreInDB.Score >= newHighScore.Score)
            {
                return false;
            }
        }
        return true;
    }
    else
    {
        throw new Exception($"Leaderboard {leaderboardName} not found.");
    }
}

    public async Task<List<HighScore>> GetHighScoreByStudentIdAsync(string UserId)
    {
        return await _dbContext.Set<HighScore>()
            .Include(h => h.User)
            .Where(h => h.User.UserId == UserId)
            .ToListAsync();
    }

    public async Task AddHighScoreAsync(HighScore highScore)
    {
        _dbContext.Set<HighScore>().Add(highScore);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateHighScoreAsync(HighScore highScore, string leaderboardName)
    {
        
        var leaderboard = await _dbContext.Set<Leaderboard>()
            .Include(l => l.HighScores)
            .FirstOrDefaultAsync(l => l.Name == leaderboardName);
        if (leaderboard != null)
        {
            HighScore? highScoreInDB = _dbContext.HighScores.FirstOrDefault(x => x.User.UserId == highScore.User.UserId);
            if (highScoreInDB != null)
            {
                if (highScoreInDB.Score < highScore.Score)
                {
                    highScoreInDB.Score = highScore.Score;
                    _dbContext.Set<HighScore>().Update(highScoreInDB);
                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                _dbContext.Set<HighScore>().Add(highScore);
                await _dbContext.SaveChangesAsync();
            }
        }   
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

    public async Task<List<HighScore>> GetHighScoresAsync()
    {
        return await _dbContext.Set<HighScore>()
            .Include(h => h.User)
            .ToListAsync();
    }


}