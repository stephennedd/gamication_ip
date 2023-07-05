
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

public class StudentQuestionService : IStudentQuestions
{
    private readonly ApplicationDbContext _dbContext;

    public StudentQuestionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

}