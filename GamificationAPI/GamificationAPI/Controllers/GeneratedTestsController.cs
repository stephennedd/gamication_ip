using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/generatedTests")]
[ApiController]
public class GeneratedTestController : ControllerBase
{
    private readonly IGeneratedTests _generatedTestService;

    public GeneratedTestController(IGeneratedTests generatedTestService)
    {
        _generatedTestService = generatedTestService;
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
}

public class GenerateTestRequest
{
    public int TestId { get; set; }
    public int StudentId { get; set; }
    public int NumberOfQuestions { get; set; }
}