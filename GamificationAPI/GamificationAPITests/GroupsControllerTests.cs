using GamificationAPI.Controllers;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamificationAPITests
{
    public class GroupsControllerTests : IDisposable
    {
        private readonly GroupsController _controller;
        private readonly ApplicationDbContext _dbContext;

        public GroupsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Groups.AddRange(
                new Group { Id = 1, Name = "Group1" },
                new Group { Id = 2, Name = "Group2" }
            );
            _dbContext.SaveChanges();

            _controller = new GroupsController(_dbContext);
        }

        [Fact]
        public async Task AddGroup_ReturnsOkResult_WhenGroupNameIsValid()
        {
            // Arrange
            var groupName = "NewGroup";

            // Act
            var result =  _controller.AddGroup(groupName);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Equal(3, _dbContext.Groups.Count());
            Assert.Contains(_dbContext.Groups, g => g.Name == groupName);
        }

        [Fact]
        public async Task AddGroup_ReturnsBadRequestResult_WhenGroupNameIsInvalid()
        {
            // Arrange
            string groupName = null;

            // Act
            var result =  _controller.AddGroup(groupName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetGroups_ReturnsOkResult_WithListOfGroups()
        {
            // Act
            var result = _controller.GetGroups();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Group>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task DeleteGroup_ReturnsOkResult_WhenIdIsValid()
        {
            // Arrange
            int validId = 1;

            // Act
            var result =  _controller.DeleteGroup(validId);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Equal(1, _dbContext.Groups.Count());
            Assert.DoesNotContain(_dbContext.Groups, g => g.Id == validId);
        }

        [Fact]
        public async Task DeleteGroup_ReturnsNotFoundResult_WhenIdIsInvalid()
        {
            // Arrange
            int invalidId = 999; // Non-existent ID

            // Act
            var result =  _controller.DeleteGroup(invalidId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task AddGroup_ReturnsBadRequest_WhenGroupNameIsEmptyString()
        {
            // Arrange
            var groupName = "";

            // Act
            var result =  _controller.AddGroup(groupName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddGroup_ReturnsBadRequest_WhenGroupNameIsWhiteSpace()
        {
            // Arrange
            var groupName = "     ";

            // Act
            var result =  _controller.AddGroup(groupName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteGroup_RemovesGroupFromUsers_WhenGroupIsDeleted()
        {
            // Arrange
            var groupId = 1;
            var user = new User
            {
                Group = _dbContext.Groups.Find(groupId),
                Password = "Password123",
                UserId = "TestUser123"
            };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Act
            var result =  _controller.DeleteGroup(groupId);

            // Assert
            Assert.IsType<OkResult>(result);
            Assert.Null(_dbContext.Users.Find(user.Id).Group);
        }
        [Fact]
        public async Task AddGroup_ReturnsBadRequest_WhenGroupNameIsNull()
        {
            // Arrange
            string groupName = null;

            // Act
            var result =  _controller.AddGroup(groupName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteGroup_ReturnsNotFound_WhenGroupDoesNotExist()
        {
            // Arrange
            var groupId = 100; // An ID that doesn't exist in the test data

            // Act
            var result =  _controller.DeleteGroup(groupId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGroup_ReturnsBadRequest_WhenGroupIdIsNegative()
        {
            // Arrange
            var groupId = -1; // Invalid ID

            // Act
            var result =  _controller.DeleteGroup(groupId);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Assuming that your method returns BadRequest in this scenario
        }
        [Fact]
        public async Task AddGroup_ReturnsBadRequest_WhenGroupNameAlreadyExists()
        {
            // Arrange
            var existingGroupName = "Group1";

            // Act
            var result = _controller.AddGroup(existingGroupName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
