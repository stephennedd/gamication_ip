
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<TestDto> CreateTest(TestDto test)
    {

        var newTest = new Test
        {
            Title = test.Title,
            ImageUrl = test.ImageUrl,
            Description = test.Description,
            TimeSeconds = test.TimeSeconds
        };

        _dbContext.Set<Test>().Add(newTest);
        await _dbContext.SaveChangesAsync();

        foreach (var question in test.Questions)
        {
            var newQuestion = new Question
            {
                QuestionText = question.QuestionText,
                CorrectAnswer = question.CorrectAnswer,
                SelectedAnswer = question.SelectedAnswer,
                TestId = newTest.Id
            };

            _dbContext.Set<Question>().Add(newQuestion);
            await _dbContext.SaveChangesAsync();

            foreach (var answer in question.Answers)
            {
                var newAnswer = new Answer
                {
                    Identifier = answer.Identifier,
                    AnswerText = answer.AnswerText,
                    QuestionId = newQuestion.Id

                };

                _dbContext.Set<Answer>().Add(newAnswer);
                await _dbContext.SaveChangesAsync();
            }
        }
        return test;
    }
    public async Task<QuestionDto> AddQuestionToTest(int testId, QuestionDto questionDto)
    {
        var test = await _dbContext.Tests.FindAsync(testId);

        if (test == null)
        {
            throw new BadHttpRequestException("The test with given testId does not exist");
        }

        var newQuestion = new Question
        {
            QuestionText = questionDto.QuestionText,
            CorrectAnswer = questionDto.CorrectAnswer,
            SelectedAnswer = questionDto.SelectedAnswer,
            TestId = testId
        };

        _dbContext.Questions.Add(newQuestion);
        await _dbContext.SaveChangesAsync();

        foreach (var answerDto in questionDto.Answers)
        {
            var newAnswer = new Answer
            {
                Identifier = answerDto.Identifier,
                AnswerText = answerDto.AnswerText,
                QuestionId = newQuestion.Id
            };

            _dbContext.Answers.Add(newAnswer);
        }

        await _dbContext.SaveChangesAsync();

        return questionDto;
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
