
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

public class StudentAnswerService : IStudentAnswers
{
    private readonly ApplicationDbContext _dbContext;
 
    public StudentAnswerService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

}
