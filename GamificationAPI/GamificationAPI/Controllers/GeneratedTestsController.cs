using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

[Route("api/generatedTests")]
[ApiController]
public class GeneratedTestController : ControllerBase
{
    private readonly IGeneratedTests _generatedTestService;
    private readonly ApplicationDbContext _dbContext;
    public GeneratedTestController(IGeneratedTests generatedTestService, ApplicationDbContext dbContext)
    {
        _generatedTestService = generatedTestService;
        _dbContext = dbContext; 
    }

    // GET: api/scoreboard
    [HttpGet]
    public async Task<IActionResult> GetGeneratedTests()
    {
        var generatedTests = await _generatedTestService.GetGeneratedTests();
        return Ok(generatedTests);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGeneratedTestById(int id)
    {
        var generatedTest = await _generatedTestService.GetGeneratedTestById(id);
        return Ok(generatedTest);
    }
    [HttpPost]
    //[FromBody] GenerateTestRequest requestBody
    public async Task<IActionResult> GenerateTestAsync([FromBody] GenerateTestRequest requestBody)
    {
        int testId = requestBody.TestId;
        int studentId = requestBody.StudentId;
        int numberOfQuestions = requestBody.NumberOfQuestions;

        var generatedTest = await _generatedTestService.GenerateTest(studentId,testId,numberOfQuestions);
        return Ok("Test was generated");
    }

    [HttpGet("{studentId}/{testId}")]
    public async Task<ActionResult<GeneratedTestDto>> GetGeneratedTest(int studentId, int testId)
    {
        var generatedTest = await _dbContext.GeneratedTest
        .Include(gt => gt.Test)
        .Include(gt => gt.Test.Questions)
        .ThenInclude(q => q.Answers)
        .FirstOrDefaultAsync(gt => gt.StudentId == studentId && gt.TestId == testId);

        if (generatedTest == null)
        {
            return NotFound();
        }

        var studentQuestions = await _dbContext.StudentQuestions
            .Include(sq => sq.Question)
            .Where(sq => sq.GeneratedTestId == generatedTest.Id)
            .OrderBy(sq => sq.Id)
            .ToListAsync();

        var generatedTestDto = new GeneratedTestDto
        {
            Id = generatedTest.Test.Id,
            Title = generatedTest.Test.Title,
            ImageUrl = generatedTest.Test.ImageUrl,
            Description = generatedTest.Test.Description,
            TimeSeconds = generatedTest.Test.TimeSeconds,
            Questions = studentQuestions.Select(sq => new GeneratedQuestionDto
            {
                Id = sq.Question.Id,
                QuestionText = sq.Question.QuestionText,
                CorrectAnswer = sq.Question.CorrectAnswer,
                SelectedAnswer = sq.Question.SelectedAnswer,
                Answers = sq.Question.Answers.Select(a => new GeneratedAnswerDto
                {
                    Id = a.Id,
                    Identifier = a.Identifier,
                    AnswerText = a.AnswerText
                }).ToList()
            }).ToList()
        };

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(generatedTestDto, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }
}

public class GenerateTestRequest
{
    public int TestId { get; set; }
    public int StudentId { get; set; }
    public int NumberOfQuestions { get; set; }
}

public class GeneratedTestDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public int TimeSeconds { get; set; }
    public List<GeneratedQuestionDto> Questions { get; set; }
}

public class GeneratedQuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string CorrectAnswer { get; set; }
    public string SelectedAnswer { get; set; }
    public List<GeneratedAnswerDto> Answers { get; set; }
}

public class GeneratedAnswerDto
{
    public int Id { get; set; }
    public string Identifier { get; set; }
    public string AnswerText { get; set; }
}