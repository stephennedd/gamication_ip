using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GamificationAPITests
{
    public class TokensControllerTests
    {
        private readonly TokensController _controller;
        private readonly Mock<IUsers> _mockUserService;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;

        public TokensControllerTests()
        {
            _mockUserService = new Mock<IUsers>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;
            _dbContext = new ApplicationDbContext(options);

            var inMemorySettings = new Dictionary<string, string> {
        {"AppSettings:Token", "ThisKeyIsLongEnoughForHmacSha512"} // Ensure this string has 16+ characters
    };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _controller = new TokensController(_config, _mockUserService.Object, _dbContext);
        }

        [Fact]
        public void CreateToken_ReturnsBadRequest_WhenUserCredentialsIsNull()
        {
            var result = _controller.CreateToken(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateToken_ReturnsBadRequest_WhenUserIdIsEmpty()
        {
            var userCredentials = new UserCredentials { UserId = "", Password = "password" };
            var result = _controller.CreateToken(userCredentials);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        // Repeat similar tests for other conditions

        [Fact]
        public void CreateToken_ReturnsOkResult_WhenUserExistsAndPasswordIsCorrect()
        {
            var userCredentials = new UserCredentials { UserId = "userId", Password = "password" };

            var user = new User
            {
                UserId = userCredentials.UserId,
                Password = BCrypt.Net.BCrypt.HashPassword(userCredentials.Password),
                Role = new Role { Name = "role" }
            };

            _mockUserService.Setup(service => service.GetUserById(It.IsAny<string>())).Returns(user);

            var result = _controller.CreateToken(userCredentials);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsBadRequest_WhenAuthorizationHeaderIsMissing()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "";
            _controller.ControllerContext.HttpContext = context;

            // Act
            var result = await _controller.RefreshTokenAsync();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Authorization header is missing.", badRequestResult.Value);
        }

        // Repeat similar tests for other conditions

        [Fact]
        public async Task RefreshTokenAsync_ReturnsOkResult_WhenUserExistsAndTokenIsValid()
        {
            var user = new User
            {
                UserId = "userId",
                Role = new Role { Name = "role" }
            };

            _mockUserService.Setup(service => service.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Here we add an Authorization Header to the controller's HttpContext
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer testToken";

            // Here we add Claims to the controller's User Identity
            _controller.ControllerContext.HttpContext.User.AddIdentity(
                new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.UserId),
                new Claim(ClaimTypes.Role, user.Role.Name)
                }));

            var result = await _controller.RefreshTokenAsync();

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public void CreateToken_ReturnsBadRequest_WhenUserIdIsNull()
        {
            // Arrange
            var credentials = new UserCredentials { UserId = null, Password = "test" };

            // Act
            var result = _controller.CreateToken(credentials);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateToken_ReturnsBadRequest_WhenPasswordIsNull()
        {
            // Arrange
            var credentials = new UserCredentials { UserId = "test", Password = null };

            // Act
            var result = _controller.CreateToken(credentials);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateToken_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var credentials = new UserCredentials { UserId = "nonexistent", Password = "test" };
            _mockUserService.Setup(s => s.GetUserById(It.IsAny<string>())).Returns((User)null);

            // Act
            var result = _controller.CreateToken(credentials);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateToken_ReturnsBadRequest_WhenPasswordIsIncorrect()
        {
            // Arrange
            var credentials = new UserCredentials { UserId = "test", Password = "wrong" };
            var user = new User { UserId = "test", Password = "test" };
            _mockUserService.Setup(s => s.GetUserById(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.CreateToken(credentials);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsBadRequest_WhenUserIdIsNull()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer token";
            _controller.ControllerContext.HttpContext = context;

            // Act
            var result = await _controller.RefreshTokenAsync();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer token";
            _controller.ControllerContext.HttpContext = context;
            _mockUserService.Setup(s => s.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.RefreshTokenAsync();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
