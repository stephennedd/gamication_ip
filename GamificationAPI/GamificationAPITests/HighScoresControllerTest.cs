using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System;
using GamificationAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using GamificationAPI.Context;

namespace GamificationAPITests
{
    public class HighScoresControllerTest
    {
        private HighScoresController _controller;
        private Mock<ILeaderboards> _leaderboardService;
        private Mock<IHighScores> _highScoreService;
        private Mock<IUsers> _userService;
        private ApplicationDbContext _context;

        public HighScoresControllerTest()
        {
            // Setup DbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            SeedDatabase();

            // Setup Mock Services
            _leaderboardService = new Mock<ILeaderboards>();
            _highScoreService = new Mock<IHighScores>();
            _userService = new Mock<IUsers>();

            _controller = new HighScoresController(_leaderboardService.Object, _highScoreService.Object, _userService.Object);
        }

        private void SeedDatabase()
        {
            // Add mock data to the database here.
            // Make sure to add a user with ID "ExistingUser"
        }

        private string GenerateMockToken(string userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(userId));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private void SetupControllerUser(HighScoresController controller, string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, userId),
            new Claim("Authorization", GenerateMockToken(userId)) // Add the token as a claim
            }));

            var httpContext = new DefaultHttpContext() { User = user };
            httpContext.Request.Headers["Authorization"] = "Bearer " + GenerateMockToken(userId); // Add the token as a Bearer token

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }
        public void SetupTestEnvironment(string userId, string leaderboardName)
        {
            // Arrange mock services
            _leaderboardService.Setup(x => x.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync(new Leaderboard());
            _highScoreService.Setup(x => x.CheckIfItsHighScore(It.IsAny<HighScore>(), leaderboardName)).ReturnsAsync(true);
            _userService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(new User { UserId = userId });
            _userService.Setup(x => x.UserExistsAsync(userId)).ReturnsAsync(true);
            _leaderboardService.Setup(x => x.CheckIfStudentHasHighScoreInLeadeboard(userId, leaderboardName)).ReturnsAsync(false);
            _leaderboardService.Setup(x => x.AddHighScoreAsync(It.IsAny<HighScore>(), leaderboardName)).ReturnsAsync(true);
            _highScoreService.Setup(x => x.UpdateMainLeaderboard(userId)).ReturnsAsync(true);

            // Arrange the User property of the controller with a ClaimsPrincipal that has the necessary claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userId)
    };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }
        [Fact]
        public async Task AddHighScoreToLeaderboard_ShouldReturnBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            SetupControllerUser(_controller, "NonexistentUser");

            // Act
            var result = await _controller.AddHighScoreToLeaderboard(100, "Leaderboard1");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddHighScoreToLeaderboard_ShouldReturnBadRequest_WhenLeaderboardNameIsNull()
        {
            // Arrange
            SetupTestEnvironment("ExistingUser", "Leaderboard1");

            // Act
            var result = await _controller.AddHighScoreToLeaderboard(100, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddHighScoreToLeaderboard_ShouldReturnBadRequest_WhenLeaderboardNameIsMain()
        {
            // Arrange
            SetupTestEnvironment("ExistingUser", "Leaderboard1");

            // Act
            var result = await _controller.AddHighScoreToLeaderboard(100, "Main");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddHighScoreToLeaderboard_ShouldReturnBadRequest_WhenUpdateMainLeaderboardFails()
        {
            // Arrange
            SetupTestEnvironment("ExistingUser", "Leaderboard1");
            _highScoreService.Setup(x => x.UpdateMainLeaderboard(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = await _controller.AddHighScoreToLeaderboard(100, "Leaderboard1");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddHighScoreToLeaderboard_ShouldReturnOk_WhenHighScoreIsAddedAndMainLeaderboardIsUpdated()
        {
            // Arrange
            SetupTestEnvironment("ExistingUser", "Leaderboard1");
            _highScoreService.Setup(x => x.UpdateMainLeaderboard(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.AddHighScoreToLeaderboard(100, "Leaderboard1");

            Assert.Null(result);
            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}