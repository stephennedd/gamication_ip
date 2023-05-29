using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/tests")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestDto>> GetTest(int id)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
        {
            return NotFound();
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
}

public class TestDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public int TimeSeconds { get; set; }
    public List<QuestionDto> Questions { get; set; }
}

public class QuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string CorrectAnswer { get; set; }
    public string SelectedAnswer { get; set; }
    public List<AnswerDto> Answers { get; set; }
}

public class AnswerDto
{
    public int Id { get; set; }
    public string Identifier { get; set; }
    public string AnswerText { get; set; }
}

