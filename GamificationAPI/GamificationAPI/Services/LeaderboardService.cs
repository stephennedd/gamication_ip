using BulkyBookWeb.Models;
using GamificationAPI.Models;
using Microsoft.EntityFrameworkCore;

public class LeaderboardService
{
    private readonly DbContext _dbContext;

    public LeaderboardService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Leaderboard>> GetLeaderboardsAsync()
    {
        return await _dbContext.Set<Leaderboard>()
            .Include(l => l.HighScores)
            .ToListAsync();
    }

    public async Task<Leaderboard> GetLeaderboardByNameAsync(string name)
    {
        return await _dbContext.Set<Leaderboard>()
            .Include(l => l.HighScores)
            .FirstOrDefaultAsync(l => l.Name == name);
    }

    public async Task AddHighScoreAsync(HighScore highScore, string leaderboardName)
    {
        var leaderboard = await GetLeaderboardByNameAsync(leaderboardName);

        if (leaderboard == null)
        {
            leaderboard = new Leaderboard { Name = leaderboardName };
            _dbContext.Set<Leaderboard>().Add(leaderboard);
        }

        leaderboard.HighScores.Add(highScore);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLeaderboardAsync(Leaderboard leaderboard)
    {
        _dbContext.Set<Leaderboard>().Update(leaderboard);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteLeaderboardAsync(string name)
    {
        var leaderboard = await GetLeaderboardByNameAsync(name);

        if (leaderboard != null)
        {
            _dbContext.Set<Leaderboard>().Remove(leaderboard);
            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task CreateLeaderboardAsync(string name)
    {
        var leaderboard = await GetLeaderboardByNameAsync(name);
        if (leaderboard != null)
        {
            return;
        }
        else 
        {
            leaderboard = new Leaderboard { Name = name };
            _dbContext.Set<Leaderboard>().Add(leaderboard);
            await _dbContext.SaveChangesAsync();
        }
    }
    public async Task<bool> CheckIfStudentHasHighScoreInLeadeboard(Student student, string name)
    {
        var leaderboard = await GetLeaderboardByNameAsync(name);
        if (leaderboard != null)
        {
                 var x =    await _dbContext.Set<HighScore>()
                    .Include(h => h.Student)
                    .Where(h => h.Student.Id == student.Id)
                    .ToListAsync();
            if (x != null)
            {
                return true;
            }
           
            
        }
        return false;
    } 
}