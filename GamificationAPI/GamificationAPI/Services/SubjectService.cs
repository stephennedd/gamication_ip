
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using System.Threading.Tasks;



public class SubjectService : ISubjects
{
    private readonly ApplicationDbContext _dbContext;

    public SubjectService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Subject>> GetSubjects()
    {
        var subjects = await _dbContext.Subjects
            .Include(s => s.Test)
                .ThenInclude(t => t.Questions)
                    .ThenInclude(q => q.Answers)
                    .ToListAsync();

        if (subjects == null)
        {
            throw new NotFoundException();
        }

        return subjects;
    }

}
