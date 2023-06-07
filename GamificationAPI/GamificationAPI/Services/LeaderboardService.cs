using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.EntityFrameworkCore;

public class LeaderboardService : ILeaderboards
{
    private readonly ApplicationDbContext _dbContext;

    public LeaderboardService(ApplicationDbContext dbContext)
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

    public async Task<bool> DeleteLeaderboardAsync(string name)
    {
        var leaderboard = await GetLeaderboardByNameAsync(name);

        if (leaderboard != null)
        {
            _dbContext.Leaderboards.Remove(leaderboard);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> CreateLeaderboardAsync(string name)
    {
        var leaderboard = await GetLeaderboardByNameAsync(name);
        if (leaderboard != null || string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        else 
        {
            leaderboard = new Leaderboard { Name = name };
            _dbContext.Set<Leaderboard>().Add(leaderboard);
            await _dbContext.SaveChangesAsync();
            return true;
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

    public Task UpdateLeaderboardAsync(string name, string newName, string newDescription, string newImageURL)
    {
        throw new NotImplementedException();
    }
}