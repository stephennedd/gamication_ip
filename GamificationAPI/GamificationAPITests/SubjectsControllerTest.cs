using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
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
        public SubjectsControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") // Use an in-memory database for testing
            .Options;

            _dbContext = new ApplicationDbContext(options);

            _mockSubjectsService = new Mock<ISubjects>();

            _controller = new SubjectController(_mockSubjectsService.Object, _dbContext);
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
    }
}
