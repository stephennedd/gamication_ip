using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Controllers;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace GamificationAPITests
{
    public class UsersControllerTests
    {
        private UsersController _controller;
        private DbContextOptions<ApplicationDbContext> _options;
        private Mock<IUsers> _mockUserService;
        private Mock<IEmails> _mockEmailService;
        private Mock<IBadges> _mockBadgeService;
        public UsersControllerTests()
        {
            // Setup an in memory database instead of mocking it
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GamificationTestDatabase")
                .Options;
            var context = new ApplicationDbContext(_options);
            _mockUserService = new Mock<IUsers>();
            _mockEmailService = new Mock<IEmails>();
            _mockBadgeService = new Mock<IBadges>();
            _controller = new UsersController(context, _mockUserService.Object, _mockEmailService.Object, _mockBadgeService.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkResult_WhenUsersExist()
        {
            // Arrange
            var users = new List<User> { new User { UserId = "1" }, new User { UserId = "2" } };
            _mockUserService.Setup(service => service.GetUsersAsync()).ReturnsAsync(users);


            // Assert
            var result = await _controller.GetAllUsers();

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("application/json", contentResult.ContentType);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockUserService.Setup(svc => svc.GetUsersAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _mockUserService.Setup(service => service.UserExistsAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        /*
        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1, Name = "Alice" }, new User { Id = 2, Name = "Bob" } };
            _mockUserService.Setup(svc => svc.GetUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(users, returnedUsers);
        }
        [Fact]
        public async Task GetAllUsers_ReturnsNoContent_WhenNoUsersExist()
        {
            // Arrange
            _mockUserService.Setup(svc => svc.GetUsersAsync()).ReturnsAsync(new List<User>());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        */
        [Fact]
        public async Task CreateStudent_ReturnsBadRequest_WhenUserExists()
        {
            // Arrange
            var userCredentials = new UserRegister { UserId = "12345", Name = "John", Surname = "Doe", Password = "password" };
            var mockUserService = new Mock<IUsers>();
            mockUserService.Setup(service => service.UserExistsAsync(userCredentials.UserId))
                .ReturnsAsync(true);  // Simulate a user already exists with this UserId
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);

            // Act
            var result = await controller.CreateStudent(userCredentials);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);  // Expecting a BadRequest response
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenUserIdsDontMatch()
        {
            // Arrange
            var userId = "12345";
            var user = new User { UserId = "54321", Name = "John", Surname = "Doe" }; // User with a different UserId
            var mockUserService = new Mock<IUsers>();
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);

            // Act
            var result = await controller.UpdateUser(userId, user);

            // Assert
            Assert.IsType<BadRequestResult>(result);  // Expecting a BadRequest response
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "12345";
            var mockUserService = new Mock<IUsers>();
            mockUserService.Setup(service => service.UserExistsAsync(userId))
                .ReturnsAsync(false);  // Simulate no user exists with this UserId
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);

            // Act
            var result = await controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);  // Expecting a NotFound response
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "12345";
            var user = new User { UserId = "12345", Name = "John", Surname = "Doe" };
            var mockUserService = new Mock<IUsers>();
            mockUserService.Setup(service => service.UserExistsAsync(userId))
                .ReturnsAsync(false);  // Simulate no user exists with this UserId
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);

            // Act
            var result = await controller.UpdateUser(userId, user);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);  // Expecting a NotFound response
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeleted()
        {
            // Arrange
            var userId = "12345";
            var mockUserService = new Mock<IUsers>();
            mockUserService.Setup(service => service.UserExistsAsync(userId))
                .ReturnsAsync(true);  // Simulate a user exists with this UserId
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);
            // Act
            var result = await controller.DeleteUser(userId);
            // Assert
            Assert.IsType<NotFoundResult>(result);  // Expecting a NoContent response
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenUserIsUpdated()
        {
            // Arrange
            var userId = "12345";
            var user = new User { UserId = "12345", Name = "John", Surname = "Doe" };
            var mockUserService = new Mock<IUsers>();
            mockUserService.Setup(service => service.UserExistsAsync(userId))
                .ReturnsAsync(true);  // Simulate a user exists with this UserId
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);
            // Act
            var result = await controller.UpdateUser(userId, user);
            // Assert
            Assert.IsType<NoContentResult>(result);  // Expecting a NoContent response
        }


        [Fact]
        public async Task CreateStudent_ReturnsCreatedResponse_WhenUserIsCreated()
        {
            // Arrange
            var userCredentials = new UserRegister { UserId = "12345", Name = "John", Surname = "Doe", Password = "password" };
            var mockUserService = new Mock<IUsers>();
            mockUserService.Setup(service => service.UserExistsAsync(userCredentials.UserId))
                .ReturnsAsync(false);  // Simulate no user exists with this UserId
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);
            // Act
            var result = await controller.CreateStudent(userCredentials);
            // Assert
            Assert.IsType<CreatedAtActionResult>(result);  // Expecting a Created response
        }
        [Fact]
        public async Task CreateStudent_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var userCredentials = new UserRegister { UserId = "12345", Name = "John", Surname = "Doe", Password = "password" };
            var mockUserService = new Mock<IUsers>();
            mockUserService.Setup(service => service.UserExistsAsync(userCredentials.UserId))
                .ReturnsAsync(false);  // Simulate no user exists with this UserId
            var mockEmailService = new Mock<IEmails>();
            var mockBadgeService = new Mock<IBadges>();
            var context = new ApplicationDbContext(_options);
            var controller = new UsersController(context, mockUserService.Object, mockEmailService.Object, mockBadgeService.Object);
            controller.ModelState.AddModelError("error", "some error");  // Simulate an invalid model state
                                                                         // Act
            var result = await controller.CreateStudent(userCredentials);
            // Assert
            Assert.IsType<BadRequestResult>(result);  // Expecting a BadRequest response
        }






    }
}