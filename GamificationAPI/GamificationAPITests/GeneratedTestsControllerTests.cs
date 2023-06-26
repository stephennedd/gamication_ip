using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GamificationAPITests
{
    public class GeneratedTestsControllerTest
    {
        private readonly GeneratedTestController _controller;
        private readonly Mock<IGeneratedTests> _mockGeneratedTestService;
        private readonly ApplicationDbContext _dbContext;

        public GeneratedTestsControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _mockGeneratedTestService = new Mock<IGeneratedTests>();
            _controller = new GeneratedTestController(_mockGeneratedTestService.Object, _dbContext);
        }

        [Fact]
        public async Task GetGeneratedTests_ReturnsListOfTests()
        {
            // Arrange
            var generatedTests = new List<GeneratedTest>
            {
                new GeneratedTest { Id = 1, StudentId = 1, TestId = 1 },
                new GeneratedTest { Id = 2, StudentId = 2, TestId = 2 },
                new GeneratedTest { Id = 3, StudentId = 1, TestId = 3 }
            };

            _mockGeneratedTestService.Setup(service => service.GetGeneratedTests()).ReturnsAsync(generatedTests);

            // Act
            var result = await _controller.GetGeneratedTests();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedTests = actionResult.Value as List<GeneratedTest>;

            // Assert
            Assert.Equal(generatedTests.Count, returnedTests.Count);
        }

        [Fact]
        public async Task GetGeneratedTestById_ReturnsTest()
        {
            // Arrange
            int testId = 1;
            var generatedTest = new GeneratedTest { Id = testId, StudentId = 1, TestId = 1 };

            _mockGeneratedTestService.Setup(service => service.GetGeneratedTestById(testId)).ReturnsAsync(generatedTest);

            // Act
            var result = await _controller.GetGeneratedTestById(testId);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedTest = actionResult.Value as GeneratedTest;

            // Assert
            Assert.Equal(generatedTest.Id, returnedTest.Id);
            Assert.Equal(generatedTest.StudentId, returnedTest.StudentId);
            Assert.Equal(generatedTest.TestId, returnedTest.TestId);
        }

        [Fact]
        public async Task GenerateTestAsync_ReturnsGeneratedTestId()
        {
            // Arrange
            var requestBody = new GenerateTestRequest
            {
                TestId = 1,
                StudentId = 1,
                NumberOfQuestions = 10
            };

            var generatedTest = new GeneratedTest { Id = 1 };

            _mockGeneratedTestService.Setup(service =>
                service.GenerateTest(requestBody.StudentId, requestBody.TestId, requestBody.NumberOfQuestions))
                .ReturnsAsync(generatedTest);

            // Act
            var result = await _controller.GenerateTestAsync(requestBody);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedTestId = (int)actionResult.Value;

            // Assert
            Assert.Equal(generatedTest.Id, returnedTestId);
        }

        [Fact]
        public async Task SaveStudentAnswer_ReturnsResponse()
        {
            // Arrange
            int studentQuestionId = 1;
            var requestBody = new GenerateUpdateStudentAnswer { AnswerId = 1 };
            var response = "Answer saved successfully";

            _mockGeneratedTestService.Setup(service =>
                service.SaveStudentAnswer(studentQuestionId, requestBody.AnswerId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.SaveStudentAnswer(studentQuestionId, requestBody);

            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var returnedResponse = actionResult.Value;

            // Assert
            Assert.Equal(response, returnedResponse);
        }

        [Fact]
        public async Task CalculateStudentResult_ReturnsResultPercentage()
        {
            // Arrange
            int studentId = 1;
            int generatedTestId = 1;
            double resultPercentage = 80.5;

            _mockGeneratedTestService.Setup(service =>
                service.CalculateStudentResult(studentId, generatedTestId))
                .ReturnsAsync(resultPercentage);

            // Act
            var result = await _controller.CalculateStudentResult(studentId, generatedTestId);

            var actionResult = Assert.IsType<ActionResult<Double>>(result);
            var returnedPercentage = (double)actionResult.Value;

            // Assert
            Assert.Equal(resultPercentage, returnedPercentage);
        }
    }
}
