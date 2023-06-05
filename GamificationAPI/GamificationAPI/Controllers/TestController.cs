using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;


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
        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(questions, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }

    [HttpPost]
    public async Task<IActionResult> CreateTest([FromBody] TestDto test)
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

        return Ok();
    }
    
}

public class CreateTestRequest
{
 
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

