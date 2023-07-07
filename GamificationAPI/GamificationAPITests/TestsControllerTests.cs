using GamificationAPI.Interfaces;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;


namespace GamificationAPITests
{
    public class TestsControllerTest
    {
        private readonly TestController _controller;
       // private readonly Mock<DbSet<Test>> _mockSet;
        private Mock<ITests> _mockTestService;
        private readonly ApplicationDbContext _dbContext;
        public TestsControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") // Use an in-memory database for testing
            .Options;

            _dbContext = new ApplicationDbContext(options);

            _mockTestService = new Mock<ITests>();

            _controller = new TestController(_mockTestService.Object, _dbContext);
        }

        [Fact]
        public async Task GetTest_ReturnsTest()
        {
            // Arrange
            int testId = 1;
            var test = new TestDto { Id = testId, Title = "Test 1" };
            _mockTestService.Setup(service => service.GetTestByIdAsync(testId)).ReturnsAsync(test);

            // Act
            var result = await _controller.GetTest(testId);

            // Assert
            var actionResult = Assert.IsType<ContentResult>(result.Result);
            var content = actionResult.Content;
            var returnedTest = JsonConvert.DeserializeObject<TestDto>(content);

            Assert.Equal(test.Id, returnedTest.Id);
            Assert.Equal(test.Title, returnedTest.Title);
        }

        [Fact]
        public async Task GetTest_ReturnsNotFound()
        {
            // Arrange
            int testId = 1;
            TestDto test = null;
            _mockTestService.Setup(service => service.GetTestByIdAsync(testId)).ReturnsAsync(test);

            // Act
            var result = await _controller.GetTest(testId);

            // Assert
            var actionResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, actionResult.StatusCode);
        }

        [Fact]
        public async Task GetTestQuestions_ReturnsTestQuestions()
        {
            // Arrange
            int testId = 1;
            var questions = new List<QuestionDto>
            {
                new QuestionDto { Id = 1, QuestionText = "Question 1" },
                new QuestionDto { Id = 2, QuestionText = "Question 2" }
            };
            _mockTestService.Setup(service => service.GetQuestionsByTestIdAsync(testId)).ReturnsAsync(questions);

            // Act
            var result = await _controller.GetTestQuestions(testId);

            // Assert
            var actionResult = Assert.IsType<ContentResult>(result.Result);
            var content = actionResult.Content;
            var returnedQuestions = JsonConvert.DeserializeObject<List<QuestionDto>>(content);

            Assert.Equal(questions.Count, returnedQuestions.Count);
        }

        [Fact]
        public async Task GetTestQuestions_ReturnsNotFound()
        {
            // Arrange
            int testId = 1;
            List<QuestionDto> questions = null;
            _mockTestService.Setup(service => service.GetQuestionsByTestIdAsync(testId)).ReturnsAsync(questions);

            // Act
            var result = await _controller.GetTestQuestions(testId);

            // Assert
            var actionResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, actionResult.StatusCode);
        }


        [Fact]
        public async Task CreateTest_ReturnsCreatedTest()
        {
            // Arrange
            var test = new TestDto { Title = "New Test" };
            var createdTest = new TestDto { Id = 1, Title = "New Test" };
            _mockTestService.Setup(service => service.CreateTest(test)).ReturnsAsync(createdTest);

            // Act
            var result = await _controller.CreateTest(test);

            // Assert
            var actionResult = Assert.IsType<ContentResult>(result.Result);
            var content = actionResult.Content;
            var returnedTest = JsonConvert.DeserializeObject<TestDto>(content);

            Assert.Equal(createdTest.Id, returnedTest.Id);
            Assert.Equal(createdTest.Title, returnedTest.Title);
        }

        [Fact]
        public async Task AddQuestionToTest_ReturnsAddedQuestion()
        {
            // Arrange
            int testId = 1;
            var newQuestion = new QuestionDto { QuestionText = "New Question" };
            var addedQuestion = new QuestionDto { Id = 1, QuestionText = "New Question" };
            _mockTestService.Setup(service => service.AddQuestionToTest(testId, newQuestion)).ReturnsAsync(addedQuestion);

            // Act
            var result = await _controller.AddQuestionToTest(testId, newQuestion);

            // Assert
            var actionResult = Assert.IsType<ContentResult>(result.Result);
            var content = actionResult.Content;
            var returnedQuestion = JsonConvert.DeserializeObject<QuestionDto>(content);

            Assert.Equal(addedQuestion.Id, returnedQuestion.Id);
            Assert.Equal(addedQuestion.QuestionText, returnedQuestion.QuestionText);
        }


    }
}