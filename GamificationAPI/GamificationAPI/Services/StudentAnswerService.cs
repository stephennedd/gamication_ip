
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
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
