
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
        .ThenInclude(h => h.User)
        .ThenInclude(u => u.Group)
        .Include(l => l.HighScores)
        .ThenInclude(h => h.User)
        .ThenInclude(u => u.Role)
        .Include(l => l.HighScores)
        .ThenInclude(h => h.User)
        .ThenInclude(u => u.Badges)
            .ToListAsync();
    }
    public async Task<List<Leaderboard>> GetLeaderboardsSimpleAsync()
    {
        return await _dbContext.Set<Leaderboard>()
            .ToListAsync();
    }

    public async Task<Leaderboard> GetLeaderboardByNameAsync(string name)
    {
        return await _dbContext.Set<Leaderboard>()
            .Include(l => l.HighScores)
        .ThenInclude(h => h.User)
        .ThenInclude(u => u.Group)
        .Include(l => l.HighScores)
        .ThenInclude(h => h.User)
        .ThenInclude(u => u.Role)
        .Include(l => l.HighScores)
        .ThenInclude(h => h.User)
        .ThenInclude(u => u.Badges)
        .FirstOrDefaultAsync(l => l.Name == name);
    }

    public async Task<bool> AddHighScoreAsync(HighScore highScore, string leaderboardName)
    {
        var leaderboard = await GetLeaderboardByNameAsync(leaderboardName);

        if (leaderboard == null)
        {
            return false;
        }
        await _dbContext.Set<HighScore>().AddAsync(highScore);
        await _dbContext.SaveChangesAsync();
        leaderboard.HighScores.Add(highScore);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> UpdateLeaderboardAsync(string oldName, string newName)
    {
        var leaderboard = await GetLeaderboardByNameAsync(oldName);
        if(leaderboard == null)
        { return false; }
        else
        {
            leaderboard.Name = newName;
            await _dbContext.SaveChangesAsync();
            return true;
        }
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
        var leaderboard = await GetLeaderboardByNameAsync(name.ToLower());
        if (leaderboard != null || string.IsNullOrWhiteSpace(name.ToLower()))
        {
            return false;
        }
        else
        {
            leaderboard = new Leaderboard { Name = name.ToLower() };
            _dbContext.Set<Leaderboard>().Add(leaderboard);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
   
    public async Task<bool> CheckIfStudentHasHighScoreInLeadeboard(string studentId, string name)
    {
        if (await _dbContext.Users.AnyAsync(s => s.UserId == studentId) == false)
        {
            throw new Exception("user with this id does not exist");
        }
        var leaderboard = await GetLeaderboardByNameAsync(name);
        if (leaderboard != null)
        {
            var x = leaderboard.HighScores.FirstOrDefault(h => h.User.UserId == studentId);
            if (x != null)
            {
                Console.WriteLine(x.Score);
                return true;
            }

        }
        Console.WriteLine("nima");
        return false;
    }


}