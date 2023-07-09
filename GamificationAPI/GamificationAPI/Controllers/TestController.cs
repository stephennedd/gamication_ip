
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin, Teacher, Student", Policy = "IsVerified")]
[Route("api/tests")]
[ApiController]
public class TestController : ControllerBase
{

    private readonly ITests _testService;
    private readonly ApplicationDbContext _dbContext;

    public TestController(ITests testService, ApplicationDbContext dbContext)
    {
        _testService = testService;
        _dbContext = dbContext;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TestDto>> GetTest(int id)
    {
        var test = await _testService.GetTestByIdAsync(id);

        if (test == null)
        {
            return NotFound(); // Test not found, return 404 status code
        }
        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(test, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }

    [HttpGet("{id}/questions")]
    public async Task<ActionResult<TestDto>> GetTestQuestions(int id)
    {
        var questions = await _testService.GetQuestionsByTestIdAsync(id);

        if (questions == null)
        {
            return NotFound(); // Test not found, return 404 status code
        }

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(questions, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }

    [HttpPost]
    public async Task<ActionResult<TestDto>> CreateTest([FromBody] TestDto test)
    {
        var testDto = await _testService.CreateTest(test);

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(testDto, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }

    [HttpPost("tests/{testId}/questions")]
    public async Task<ActionResult<QuestionDto>> AddQuestionToTest(int testId, [FromBody] QuestionDto newQuestionDto)
    {
        var questionDto = await _testService.AddQuestionToTest(testId, newQuestionDto);

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(questionDto, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }

}

public class TestDto
{
    public int? Id { get; set; }
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

