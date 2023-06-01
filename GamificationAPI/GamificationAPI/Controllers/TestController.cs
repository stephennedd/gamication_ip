using GamificationAPI.Interfaces;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


[Route("api/tests")]
[ApiController]
public class TestController : ControllerBase
{

    private readonly ITests _testService;

    public TestController(ITests testService)
    {
        _testService = testService;
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

