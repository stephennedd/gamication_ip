using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using GamificationAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using SendGrid.Helpers.Mail;

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
        public async Task DeleteUser_ReturnsOk_WhenUserSuccessfullyDeleted()
        {
            // Arrange
            var validUserId = "validUser";

            var userToDelete = new User
            {
                UserId = validUserId,
                Password = "password",
                Name = "Name",
                Surname = "Surname"
            };

            _mockUserService.Setup(s => s.UserExistsAsync(validUserId)).ReturnsAsync(true);
            _mockUserService.Setup(s => s.GetUserByIdAsync(validUserId)).ReturnsAsync(userToDelete);
            _mockUserService.Setup(s => s.DeleteUserAsync(validUserId)).Returns(Task.CompletedTask);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer mock_token";
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Role, "Admin")
            }));

            // Act
            var result = await _controller.DeleteUser(validUserId);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public async Task UpdateUser_ReturnsUpdatedUser_WhenUserSuccessfullyUpdated()
        {
            // Arrange
            var validUserId = "validUser";
            var userToUpdate = new User
            {
                UserId = validUserId,
                Password = "newPassword",
                Name = "New Name",
                Surname = "New Surname"
            };

            _mockUserService.Setup(s => s.UserExistsAsync(validUserId)).ReturnsAsync(true);
            _mockUserService.Setup(s => s.UpdateUserAsync(userToUpdate)).Returns(Task.CompletedTask);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer mock_token";
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, validUserId)
            }));

            // Act
            var result = await _controller.UpdateUser(validUserId, userToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userToUpdate, returnedUser);
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
        [Fact]
        public async Task GetUser_ReturnsOkResult_WhenUserExists()
        {
            // Arrange
            var userId = "user1";
            _mockUserService.Setup(svc => svc.UserExistsAsync(userId)).ReturnsAsync(true);
            _mockUserService.Setup(svc => svc.GetUserByIdAsync(userId)).ReturnsAsync(new User { UserId = userId });

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(actionResult.Value);
            Assert.Equal(userId, returnValue.UserId);
        }

        [Fact]
        public async Task CreateTeacher_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Model state is invalid");

            // Act
            var result = await _controller.CreateTeacher(new TeacherRegister(), false);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }     

        [Fact]
        public async Task CreateStudent_ReturnsBadRequestResult_WhenUserAlreadyExists()
        {
            // Arrange
            var newUser = new UserRegister { UserId = "existingUser", Name = "John", Surname = "Doe" };
            _mockUserService.Setup(s => s.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateStudent(newUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User with this ID already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task VerifyUser_ReturnsBadRequest_WhenNoAuthorizationHeader()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

            // Act
            var result = await _controller.VerifyUser("token");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenNoAuthorizationHeader()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

            // Act
            var result = await _controller.ChangePassword("newPassword");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddBadgeToUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserService.Setup(s => s.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _controller.AddBadgeToUser("nonExistentUser", 1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddGroupToUser_ReturnsBadRequest_WhenNoAuthorizationHeader()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

            // Act
            var result = await _controller.AddGroupToUser("groupName");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task VerifyUser_ReturnsOk_WhenTokenIsValid()
        {
            // Arrange
            var validToken = "validToken";
            var validUserId = "validUser";

            _mockUserService.Setup(s => s.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockUserService.Setup(s => s.VerifyUser(validUserId, validToken)).ReturnsAsync(true);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer mock_token";
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, validUserId)
            }));

            // Act
            var result = await _controller.VerifyUser(validToken);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenPasswordChangeIsSuccessful()
        {
            // Arrange
            var validUserId = "validUser";
            var newPassword = "newPassword";

            _mockUserService.Setup(s => s.UserExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            _mockUserService.Setup(s => s.ChangePasswordAsync(validUserId, newPassword)).ReturnsAsync(true);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer mock_token";
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, validUserId)
            }));

            // Act
            var result = await _controller.ChangePassword(newPassword);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddBadgeToUser_ReturnsOk_WhenUserAndBadgeExist()
        {
            // Arrange
            var validUserId = "validUser";
            var validBadgeId = 1;

            _mockUserService.Setup(s => s.UserExistsAsync(validUserId)).ReturnsAsync(true);
            _mockBadgeService.Setup(s => s.BadgeExistsAsync(validBadgeId)).ReturnsAsync(true);
            _mockBadgeService.Setup(s => s.GetBadgeAsync(validBadgeId)).ReturnsAsync(new Badge());
            _mockUserService.Setup(s => s.AddBadgeAsync(It.IsAny<Badge>(), validUserId)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddBadgeToUser(validUserId, validBadgeId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddGroupToUser_ReturnsOk_WhenUserExistsAndGroupValid()
        {
            // Arrange
            var validUserId = "validUser";
            var validGroupName = "validGroup";

            _mockUserService.Setup(s => s.UserExistsAsync(validUserId)).ReturnsAsync(true);
            _mockUserService.Setup(s => s.AddGroupToUserAsync(validUserId, validGroupName)).ReturnsAsync(true);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer mock_token";
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.Name, validUserId)
            }));

            // Act
            var result = await _controller.AddGroupToUser(validGroupName);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        

        
    }
}