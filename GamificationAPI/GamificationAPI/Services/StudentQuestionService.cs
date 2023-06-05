using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
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