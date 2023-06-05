using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;


public class TestService : ITests
{
    private readonly ApplicationDbContext _dbContext;

    public TestService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TestDto> GetTestByIdAsync(int id)
    {
        var test = await _dbContext.Tests
           .Include(t => t.Questions)
               .ThenInclude(q => q.Answers)
           .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
        {
            new BadHttpRequestException("Test does not exist");
        }

        var testDto = new TestDto
        {
            Id = test.Id,
            Title = test.Title,
            ImageUrl = test.ImageUrl,
            Description = test.Description,
            TimeSeconds = test.TimeSeconds,
            Questions = test.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                CorrectAnswer = q.CorrectAnswer,
                SelectedAnswer = q.SelectedAnswer,
                Answers = q.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Identifier = a.Identifier,
                    AnswerText = a.AnswerText
                }).ToList()
            }).ToList()
        };

        return testDto;
    }

    public async Task<List<Question>> GetQuestionsByIdsAsync(List<int> questionIds)
    {
        var questions = await _dbContext.Questions
            .Include(q => q.Answers)
            .Where(q => questionIds.Contains(q.Id))
            .ToListAsync();

        return questions;
    }

    public async Task<List<QuestionDto>> GetQuestionsByTestIdAsync(int testId)
    {
        var test = await _dbContext.Tests
        .Include(t => t.Questions)
            .ThenInclude(q => q.Answers)
        .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
        {
            throw new BadHttpRequestException("Test does not exist");
        }

        var questionDtos = test.Questions.Select(q => MapQuestionToDto(q)).ToList();

        return questionDtos;
    }

    private QuestionDto MapQuestionToDto(Question question)
    {
        var questionDto = new QuestionDto
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            CorrectAnswer = question.CorrectAnswer,
            SelectedAnswer = question.SelectedAnswer,
            Answers = question.Answers.Select(a => MapAnswerToDto(a)).ToList()
        };

        return questionDto;
    }

    private AnswerDto MapAnswerToDto(Answer answer)
    {
        var answerDto = new AnswerDto
        {
            Id = answer.Id,
            Identifier = answer.Identifier,
            AnswerText = answer.AnswerText
        };

        return answerDto;
    }

    public Task<List<Test>> GetTestsAsync()
    {
        throw new NotImplementedException();
    }
}
