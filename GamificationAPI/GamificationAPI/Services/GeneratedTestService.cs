using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<GeneratedTest> GenerateTest(int studentId,int testId, int numberOfQuestions)
    {
        var questions = await _testService.GetQuestionsByTestIdAsync(testId); // Retrieve an existing test from the database
        var student = _dbContext.Users.FirstOrDefault(s => s.Id == studentId);// Retrieve a student from the database

        // Retrieve the answered question IDs for the existing generated test from the StudentAnswers table
        

        if (questions != null && student != null)
        {
            // change it
            var existingGeneratedTest = _dbContext.GeneratedTest.OrderByDescending(gt => gt.Id).FirstOrDefault(gt => gt.StudentId == studentId && gt.TestId == testId);
            if (existingGeneratedTest != null)
            {
                var answeredQuestionCount = await _dbContext.StudentQuestions
        .CountAsync(sq => sq.GeneratedTestId == existingGeneratedTest.Id && sq.AnswerId != null);
                

                // Retrieve the total number of question IDs for the existing generated test from the StudentQuestions table
                var totalOfQuestionsOfGeneratedTest = _dbContext.StudentQuestions
                    .Count(sq => sq.GeneratedTestId == existingGeneratedTest.Id);
                 if (existingGeneratedTest != null && answeredQuestionCount < totalOfQuestionsOfGeneratedTest)
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
                    QuestionId = questionId
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

    public async Task<ActionResult<string>> SaveStudentAnswer(int studentQuestionId, int answerId)
    {
  
        var studentQuestion = await _dbContext.StudentQuestions.FindAsync(studentQuestionId);

        if (studentQuestion == null)
        {
            throw new BadHttpRequestException("Student question was not found based on provided student question id");
        }

        // Fetch the associated question for the student question
        var question = await _dbContext.Questions.FindAsync(studentQuestion.QuestionId);

        if (question == null)
        {
            throw new BadHttpRequestException("Question was not found for the provided student question ID.");
        }

        // Check if the provided answer ID belongs to the question
        var answer = await _dbContext.Answers.FindAsync(answerId);

        if (answer == null || answer.QuestionId != question.Id)
        {
            throw new BadHttpRequestException("The provided answer ID does not belong to the question associated with the student question ID.");
        }

        if (studentQuestion.AnswerId!=null)
        {
            throw new BadHttpRequestException("Answer was already saved before");
        }

        studentQuestion.AnswerId = answerId;
        await _dbContext.SaveChangesAsync();

        return "Answer was saved";
    }

    public async Task<ActionResult<Double>> CalculateStudentResult(int studentId, int generatedTestId)
    {
        var student = await _dbContext.Users.FindAsync(studentId);

        var generatedTest = await _dbContext.GeneratedTest
            .Include(gt => gt.Test)
            .FirstOrDefaultAsync(gt => gt.Id == generatedTestId && gt.StudentId == studentId);

        if (student == null || generatedTest == null)
        {
            throw new NotFoundException();
        }

        var studentQuestions = await _dbContext.StudentQuestions
            .Include(sq => sq.Question)
            .Where(sq => sq.GeneratedTestId == generatedTestId)
            .ToListAsync();

        // Check if the student has answered all the questions
        bool allQuestionsAnswered = studentQuestions.All(sq => sq.AnswerId != null);
        if (!allQuestionsAnswered)
        {
            throw new BadHttpRequestException("Not all questions have been answered by the student.");
        }


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

    public async Task<GeneratedTestDto> GetGeneratedTest(int studentId, int testId)
    {
        var generatedTest = await _dbContext.GeneratedTest
            .Include(gt => gt.Test)
            .Include(gt => gt.Test.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(gt => gt.StudentId == studentId && gt.TestId == testId);

        if (generatedTest == null)
        {
            throw new BadHttpRequestException("Generated test with provided testId and studentId does not exist");
        }

        var studentQuestions = await _dbContext.StudentQuestions
            .Include(sq => sq.Question)
            .Include(sq => sq.Question.Answers)
            .Where(sq => sq.GeneratedTestId == generatedTest.Id)
            .ToListAsync();

        var hasUnansweredQuestions = studentQuestions.Any(sq => sq.AnswerId == null);
        if (!hasUnansweredQuestions)
        {
            // Get the next generated test with unanswered questions
            generatedTest = await _dbContext.GeneratedTest
                .Include(gt => gt.Test)
                .Include(gt => gt.Test.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(gt => gt.StudentId == studentId && gt.Id > generatedTest.Id
                    && _dbContext.StudentQuestions.Any(sq => sq.GeneratedTestId == gt.Id && sq.AnswerId == null));

            if (generatedTest == null)
            {
                throw new BadHttpRequestException("No more generated tests with unanswered questions found");
            }

            studentQuestions = await _dbContext.StudentQuestions
                .Include(sq => sq.Question)
                .Include(sq => sq.Question.Answers)
                .Where(sq => sq.GeneratedTestId == generatedTest.Id)
                .ToListAsync();
        }

        var generatedTestDto = new GeneratedTestDto
        {
            Id = generatedTest.Id,
            Title = generatedTest.Test.Title,
            ImageUrl = generatedTest.Test.ImageUrl,
            Description = generatedTest.Test.Description,
            TimeSeconds = generatedTest.Test.TimeSeconds,
            Questions = studentQuestions.Select(sq => new GeneratedQuestionDto
            {
                Id = sq.Id,
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

        return generatedTestDto;
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
  