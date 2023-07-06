using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GamificationAPITests
{
    public class SubjectsControllerTest
    {
        private readonly SubjectController _controller;
       // private readonly Mock<DbSet<Subject>> _mockSet;
        private Mock<ISubjects> _mockSubjectsService;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ApplicationDbContext> _mockDbContext;


        public SubjectsControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") // Use an in-memory database for testing
            .Options;

            _dbContext = new ApplicationDbContext(options);

            _mockSubjectsService = new Mock<ISubjects>();

            _controller = new SubjectController(_mockSubjectsService.Object, _dbContext);

            _mockDbContext = new Mock<ApplicationDbContext>();
        }


        [Fact]
        public async Task GetGameNameBySubject_ReturnsGameName_WhenSubjectExists()
        {
            // Arrange
            string subjectName = "Subject 1";
            string gameName = "Game 1";

            var games = new List<Game>
    {
        new Game { GameName = gameName, Subjects = new List<Subject> { new Subject { SubjectTitle = subjectName } } },
        new Game { GameName = "Game 2", Subjects = new List<Subject> { new Subject { SubjectTitle = "Subject 2" } } },
        new Game { GameName = "Game 3", Subjects = new List<Subject> { new Subject { SubjectTitle = "Subject 3" } } }
    };

            _dbContext.Games.AddRange(games);
            await _dbContext.SaveChangesAsync();

            // Log the count of games in the context to verify the data is correctly added
            Console.WriteLine("Game count in context: " + _dbContext.Games.Count());

            // Act
            var result = await _controller.GetGameNameBySubject(subjectName);
           
            // Assert
            Assert.Equal("Game 1", result.Value);
        }

        [Fact]
        public async Task GetGameNameBySubject_ReturnsNotFound_WhenSubjectDoesNotExist()
        {
            // Arrange
            string subjectName = "Non-existent Subject";
            var games = new List<Game>
        {
            new Game { GameName = "Game 1", Subjects = new List<Subject> { new Subject { SubjectTitle = "Subject 1" } } },
            new Game { GameName = "Game 2", Subjects = new List<Subject> { new Subject { SubjectTitle = "Subject 2" } } },
            new Game { GameName = "Game 3", Subjects = new List<Subject> { new Subject { SubjectTitle = "Subject 3" } } }
        };

            _dbContext.Games.AddRange(games);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetGameNameBySubject(subjectName);
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetTestId_ReturnsTestId_WhenSubjectExists()
        {
            // Arrange
            string subjectName = "Subject 1";
            int testId = 1;

            var test = new Test
            {
                Id = testId,
                Title = "Test Title",
                Description = "Test Description",
                ImageUrl = "https://example.com/test-image.jpg"
            };

            var subject = new Subject
            {
                SubjectTitle = subjectName,
                Test = test
            };

            _dbContext.Subjects.Add(subject);
            _dbContext.Tests.Add(test);
            _dbContext.SaveChanges();

            // Act
            var result = _controller.GetTestId(subjectName);

            // Assert
            Assert.Equal(testId, result.Value);
        }


        [Fact]
        public void GetTestId_ReturnsNotFound_WhenSubjectDoesNotExist()
        {
            // Arrange
            string subjectName = "Non-existent Subject";

            // Act
            var result = _controller.GetTestId(subjectName);
            var actionResult = Assert.IsType<ActionResult<int>>(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetTestId_ReturnsNotFound_WhenSubjectHasNoTest()
        {
            // Arrange
            string subjectName = "Subject 1";

            var subject = new Subject
            {
                SubjectTitle = subjectName,
                Test = null
            };

            _dbContext.Subjects.Add(subject);
            _dbContext.SaveChanges();

            // Act
            var result = _controller.GetTestId(subjectName);
            var actionResult = Assert.IsType<ActionResult<int>>(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(actionResult.Result);

            // Assert
            Assert.Equal(404, notFoundResult.StatusCode);
        }



        [Fact]
        public async Task GetSubjects_ReturnsListOfSubjects()
        {
            // Arrange
            var subjects = new List<Subject>
             {
             new Subject { Id = 1, SubjectTitle = "Subject 1" },
             new Subject { Id = 2, SubjectTitle = "Subject 2" },
             new Subject { Id = 3, SubjectTitle = "Subject 3" }
             };

            _mockSubjectsService.Setup(service => service.GetSubjects()).ReturnsAsync(subjects);

            // Act
            var result = await _controller.GetSubjects();

            var actionResult = Assert.IsType<ContentResult>(result.Result);
            var contentResult = actionResult.Content;

            // Deserialize the response content to a list of subjects
            var returnedSubjects = JsonConvert.DeserializeObject<List<Subject>>(contentResult);

            Assert.Equal(subjects.Count, returnedSubjects.Count);
        }

        [Fact]
        public async Task AddSubject_ReturnsCreatedSubject()
        {
            // Arrange
            var newSubject = new NewSubject { SubjectTitle = "New Subject" };
            var createdSubject = new Subject { Id = 1, SubjectTitle = "New Subject" };
            _mockSubjectsService.Setup(service => service.AddSubject(newSubject)).ReturnsAsync(createdSubject);

            // Act
            var result = await _controller.AddSubject(newSubject);

            // Assert
            var actionResult = Assert.IsType<ContentResult>(result.Result);
            var content = actionResult.Content;
            var returnedSubject = JsonConvert.DeserializeObject<Subject>(content);

            Assert.Equal(createdSubject.Id, returnedSubject.Id);
            Assert.Equal(createdSubject.SubjectTitle, returnedSubject.SubjectTitle);
        }

        [Fact]
        public async Task UpdateTables_ReturnsUpdatedRootObject()
        {
            // Arrange
            var data = new RootObject { Id = 1, SubjectTitle = "Updated Subject" };
            var updatedRootObject = new RootObject { Id = 1, SubjectTitle = "Updated Subject" };
            _mockSubjectsService.Setup(service => service.UpdateTables(data)).ReturnsAsync(updatedRootObject);

            // Act
            var result = await _controller.UpdateTables(data);

            // Assert
            var actionResult = Assert.IsType<ContentResult>(result.Result);
            var content = actionResult.Content;
            var returnedRootObject = JsonConvert.DeserializeObject<RootObject>(content);

            Assert.Equal(updatedRootObject.Id, returnedRootObject.Id);
            Assert.Equal(updatedRootObject.SubjectTitle, returnedRootObject.SubjectTitle);
            // Add more assertions as needed
        }

        [Fact]
        public async Task GetSubjects_ReturnsNotFound()
        {
            // Arrange
            List<Subject> subjects = null;
            _mockSubjectsService.Setup(service => service.GetSubjects()).ReturnsAsync(subjects);

            // Act
            var result = await _controller.GetSubjects();

            // Assert
            var actionResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, actionResult.StatusCode);
        }

        [Fact]
        public async Task DeleteSubject_ReturnsOkResult_WhenSubjectExists()
        {
            // Arrange
            int subjectId = 1;
            _mockSubjectsService.Setup(service => service.DeleteSubject(subjectId)).ReturnsAsync(new Subject());

            // Act
            var result = await _controller.DeleteSubject(subjectId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteSubject_ReturnsNotFound_WhenSubjectDoesNotExist()
        {
            // Arrange
            int subjectId = 1;
            _mockSubjectsService.Setup(service => service.DeleteSubject(subjectId)).ReturnsAsync((Subject)null);

            // Act
            var result = await _controller.DeleteSubject(subjectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

    }
}
