
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

        var generatedTest = await _generatedTestService.GenerateTest(studentId.ToString(),testId,numberOfQuestions);
        return Ok(generatedTest.Id);
    }

    [HttpPost("studentQuestions/{studentQuestionId}/answer")]
    public async Task<ActionResult<string>> SaveStudentAnswer(int studentQuestionId, [FromBody] GenerateUpdateStudentAnswer requestBody)
    {
        var answerId = requestBody.AnswerId;
       
        var response = await _generatedTestService.SaveStudentAnswer(studentQuestionId,answerId);

        return response;
    }

    [HttpGet("{studentId}/{testId}")]
    public async Task<ActionResult<GeneratedTestDto>> GetGeneratedTest(string studentId, int testId)
    {
        var generatedTest = await _generatedTestService.GetGeneratedTest(studentId,testId);

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(generatedTest, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }

    [HttpGet("studentResults")]
    public async Task<ActionResult<Double>> CalculateStudentResult(string studentId, int generatedTestId)
    {
        var student = await _dbContext.Users.FindAsync(studentId);

        var generatedTest = await _dbContext.GeneratedTest
            .Include(gt => gt.Test)
            .FirstOrDefaultAsync(gt => gt.Id == generatedTestId && gt.StudentId == studentId);

        if (student == null || generatedTest == null)
        {
            return NotFound();
        }

        var studentQuestions = await _dbContext.StudentQuestions
            .Include(sq => sq.Question)
            .Where(sq => sq.GeneratedTestId == generatedTestId)
            .ToListAsync();

        // Calculate the number of correct answers
        int numberOfCorrectAnswers = 0;
        foreach (var studentQuestion in studentQuestions)
        {
            var question = studentQuestion.Question;
            var correctAnswer = await _dbContext.Answers
                .FirstOrDefaultAsync(a => a.QuestionId == question.Id && a.AnswerText == question.CorrectAnswer);

            if (correctAnswer != null && studentQuestion.AnswerId == correctAnswer.Id)
            {
                numberOfCorrectAnswers++;
            }
        }

        int totalNumberOfQuestionsPerGeneratedQuiz = studentQuestions.Count;

        // Calculate the result as a percentage
        double resultPercentage = (double)numberOfCorrectAnswers / totalNumberOfQuestionsPerGeneratedQuiz * 100;

        return resultPercentage;
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