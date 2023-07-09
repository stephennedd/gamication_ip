using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;


[Authorize(Roles = "Admin, Teacher, Student")]
[Route("api/[controller]")]
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
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetGeneratedTests()
    {
        var generatedTests = await _generatedTestService.GetGeneratedTests();
        if (generatedTests == null)
        {
            return NotFound(); // generatedTests not found, return 404 status code
        }
        return Ok(generatedTests);
    }
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGeneratedTestById(int id)
    {
        var generatedTest = await _generatedTestService.GetGeneratedTestById(id);
        if (generatedTest == null)
        {
            return NotFound(); // generatedTest not found, return 404 status code
        }
        return Ok(generatedTest);
    }

    [AllowAnonymous]
    [HttpPost]
    //[FromBody] GenerateTestRequest requestBody
    public async Task<IActionResult> GenerateTestAsync([FromBody] GenerateTestRequest requestBody)
    {
        int testId = requestBody.TestId;
        int studentId = requestBody.StudentId;
        int numberOfQuestions = requestBody.NumberOfQuestions;

        var generatedTest = await _generatedTestService.GenerateTest(studentId,testId,numberOfQuestions);
        return Ok(generatedTest.Id);
    }
    
    [AllowAnonymous]
    [HttpPost("studentQuestions/{studentQuestionId}/answer")]
    public async Task<ActionResult<string>> SaveStudentAnswer(int studentQuestionId, [FromBody] GenerateUpdateStudentAnswer requestBody)
    {
        var answerId = requestBody.AnswerId;
       
        var response = await _generatedTestService.SaveStudentAnswer(studentQuestionId,answerId);

        return response;
    }
    [AllowAnonymous]
    [HttpGet("{studentId}/{testId}")]
    public async Task<ActionResult<GeneratedTestDto>> GetGeneratedTest(int studentId, int testId)
    {
        var generatedTest = await _generatedTestService.GetGeneratedTest(studentId,testId);

        if (generatedTest == null)
        {
            return NotFound(); // generatedTest not found, return 404 status code
        }

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(generatedTest, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }
    [AllowAnonymous]
    [HttpGet("studentResults")]
    public async Task<ActionResult<Double>> CalculateStudentResult(int studentId, int generatedTestId)
    {
        var resultPrecentage = await _generatedTestService.CalculateStudentResult(studentId, generatedTestId);
        return resultPrecentage;
    }
}
public class GenerateTestRequest
{
    public int TestId { get; set; }
    public int StudentId { get; set; }
    public int NumberOfQuestions { get; set; }
}

public class GenerateUpdateStudentAnswer
{ 
    public int AnswerId { get; set; }
}