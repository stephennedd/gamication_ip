using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;

public class GeneratedTestService : IGeneratedTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITests _testService; // Reference to the ITests service

    public GeneratedTestService(ApplicationDbContext dbContext, ITests testService)
    {
        _dbContext = dbContext;
        _testService = testService;
    }

    //public async Task<Question> getQuestionsOfGeneratedTest(int studentId, int testId)
    //{
    
    //    var questions
   // }

    public async Task<GeneratedTest> GenerateTest(int studentId,int testId, int numberOfQuestions)
    {
        var questions = await _testService.GetQuestionsByTestIdAsync(testId); // Retrieve an existing test from the database
        var student = _dbContext.Students.FirstOrDefault(s => s.Id == studentId);// Retrieve a student from the database

        // Retrieve the answered question IDs for the existing generated test from the StudentAnswers table
        

        if (questions != null && student != null)
        {
            var existingGeneratedTest = _dbContext.GeneratedTest.FirstOrDefault(gt => gt.StudentId == studentId && gt.TestId == testId);
            if (existingGeneratedTest != null)
            {
                var answeredQuestionIds = _dbContext.StudentAnswers
                    .Where(sa => sa.GeneratedTestId == existingGeneratedTest.Id)
                    .Select(sa => sa.Answer.QuestionId)
                    .ToList();

                // Retrieve the total number of question IDs for the existing generated test from the StudentQuestions table
                var totalOfQuestionsOfGeneratedTest = _dbContext.StudentQuestions
                    .Count(sq => sq.GeneratedTestId == existingGeneratedTest.Id);
                 if (existingGeneratedTest != null && answeredQuestionIds.Count < totalOfQuestionsOfGeneratedTest)
                 {
                     throw new BadHttpRequestException("Test was already generated for that student. He needs to complete it before he can request another test");
                 }

            }

            var randomQuestions = questions
          .OrderBy(q => Guid.NewGuid()) // Randomize the order of questions
          .Take(numberOfQuestions); // Select a certain number of random questions, e.g., 5

          var questionIds = randomQuestions.Select(q => q.Id).ToList(); // Store the IDs of the randomly selected questions

          var generatedTest1 = new GeneratedTest
            {
                StudentId = studentId,
                TestId = testId
            };

            // Save the generated test to the database
            _dbContext.GeneratedTest.Add(generatedTest1);
            _dbContext.SaveChanges();

            // Insert rows into the StudentQuestions table
            foreach (var questionId in questionIds)
            {
                var studentQuestion = new StudentQuestion
                {
                    GeneratedTestId = generatedTest1.Id,
                    QuestionId = questionId,
                    IsAnswered = false 
                };

                _dbContext.StudentQuestions.Add(studentQuestion);
            }

            _dbContext.SaveChanges();

            return generatedTest1;
        } else
        {
            throw new BadHttpRequestException("Student/Test does not exist");
        }
    }

    public Task<GeneratedTest> GetGeneratedTestById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<GeneratedTest>> GetGeneratedTests()
    {
        throw new NotImplementedException();
    }
}