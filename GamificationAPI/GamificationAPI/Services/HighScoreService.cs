
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
            .ThenInclude(u => u.Group)
            .Include(h => h.User)
            .ThenInclude(u => u.Role)
            .Include(h => h.User)
            .ThenInclude(u => u.Badges)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
    public async Task<bool> CheckIfItsHighScore(HighScore newHighScore, string leaderboardName)
    {
        var leaderboard = await _dbContext.Set<Leaderboard>()
            .Include(l => l.HighScores)
            .FirstOrDefaultAsync(l => l.Name == leaderboardName);
        if (leaderboard == null)
        {
            throw new Exception($"Leaderboard {leaderboardName} not found.");
        }
            List<HighScore> userHS = _dbContext.HighScores.Include(l => l.Leaderboard).Where(item => item.User.UserId == newHighScore.User.UserId).ToList();
        if (userHS.Count == 0)
        {
            return true;
        }
                HighScore? highScoreInDB = userHS.FirstOrDefault(item => item.Leaderboard.Name == leaderboardName);
                if (highScoreInDB != null)
                {
                    if (highScoreInDB.Score >= newHighScore.Score)
                    {
                        return false;
                    }
                }
                return true;
            
            
        
        
    }



    public async Task<List<HighScore>> GetHighScoreByStudentIdAsync(string UserId)
    {
        return await _dbContext.Set<HighScore>()
            .Include(h => h.User)
            .ThenInclude(u => u.Group)
            .Include(h => h.User)
            .ThenInclude(u => u.Role)
            .Include(h => h.User)
            .ThenInclude(u => u.Badges)
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
            List<HighScore> userHS = _dbContext.HighScores.Include(l => l.Leaderboard).Where(item => item.User.UserId == highScore.User.UserId).ToList();
            if (userHS.Count != 0)
            {
                HighScore? highScoreInDB = userHS.FirstOrDefault(item => item.Leaderboard.Name == leaderboardName);
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
    public async Task<bool> UpdateMainLeaderboard(string userId)
    {
        try
        {
            var user = _dbContext.Users.Include(l => l.HighScores).FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return false;
            }
            List<HighScore> userHS = _dbContext.HighScores.Include(l => l.Leaderboard).Where(item => item.User.UserId == userId).ToList();

            if (userHS.Count != 0)
            {
                HighScore? highScoreInDB = userHS.FirstOrDefault(item => item.Leaderboard.Name == "main");
                int overallScore = 0;
                if (highScoreInDB != null)
                {
                    highScoreInDB.Score = 0;
                }
                    foreach (var item in user.HighScores)
                {
                    overallScore += item.Score;
                }
                if (highScoreInDB != null)
                {
                    highScoreInDB.Score = overallScore;
                    _dbContext.Set<HighScore>().Update(highScoreInDB);
                    await _dbContext.SaveChangesAsync();

                }
                else
                {
                    var highScore = new HighScore
                    {
                        Leaderboard = await _dbContext.Leaderboards.FirstOrDefaultAsync(l => l.Name == "main"),
                        Score = overallScore,
                        User = user
                    };
                    _dbContext.Set<HighScore>().Add(highScore);
                    await _dbContext.SaveChangesAsync();
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
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
            .ThenInclude(u => u.Group)
            .Include(h => h.User)
            .ThenInclude(u => u.Role)
            .Include(h => h.User)
            .ThenInclude(u => u.Badges)
            .ToListAsync();
    }



}