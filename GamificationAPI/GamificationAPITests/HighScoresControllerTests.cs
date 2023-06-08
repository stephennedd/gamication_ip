using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;

using GamificationAPI.Models;
using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
/*
namespace GamificationAPITests
{
    public class HighScoresControllerTests
    {
        
        [Fact]
        public async Task Post_AddHighScoreToLeaderboard_ReturnsOK_WhenHighScoreIsAdded()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var mockHighScoresService = new Mock<IHighScores>();
            var mockStudentsService = new Mock<IUsers>();

            var highScore = new HighScore { Student = new User { Id = 1 }, Score = 50 };
            var leaderboardName = "TestLeaderboard";

            mockLeaderboardsService.Setup(s => s.CheckIfStudentHasHighScoreInLeadeboard(highScore.Student, leaderboardName)).ReturnsAsync(false);
            mockHighScoresService.Setup(s => s.CheckIfItsHighScore(highScore, leaderboardName)).ReturnsAsync(true);

            var controller = new HighScoresController(mockLeaderboardsService.Object, mockHighScoresService.Object, mockStudentsService.Object);

            // Act
            var result = await controller.AddHighScoreToLeaderboard(highScore, leaderboardName);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Post_AddHighScoreToLeaderboard_ReturnsBadRequest_WhenHighScoreIsNotHighScore()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var mockHighScoresService = new Mock<IHighScores>();
            var mockStudentsService = new Mock<IUsers>();

            var highScore = new HighScore { Student = new User { Id = 1 }, Score = 50 };
            var leaderboardName = "TestLeaderboard";

            mockLeaderboardsService.Setup(s => s.CheckIfStudentHasHighScoreInLeadeboard(highScore.Student, leaderboardName)).ReturnsAsync(true);
            mockHighScoresService.Setup(s => s.CheckIfItsHighScore(highScore, leaderboardName)).ReturnsAsync(false);

            var controller = new HighScoresController(mockLeaderboardsService.Object, mockHighScoresService.Object, mockStudentsService.Object);

            // Act
            var result = await controller.AddHighScoreToLeaderboard(highScore, leaderboardName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This is not High Score", (result as BadRequestObjectResult).Value);
        }
        [Fact]
        public async Task Delete_DeleteHighScoreById_ReturnsOK_WhenHighScoreIsDeleted()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var mockHighScoresService = new Mock<IHighScores>();
            var mockStudentsService = new Mock<IUsers>();

            var highScoreId = 1;

            mockHighScoresService.Setup(s => s.DeleteHighScoreAsync(highScoreId)).Returns(Task.CompletedTask);

            var controller = new HighScoresController(mockLeaderboardsService.Object, mockHighScoresService.Object, mockStudentsService.Object);

            // Act
            var result = await controller.DeleteHighScoreById(highScoreId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_DeleteHighScoreById_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var mockHighScoresService = new Mock<IHighScores>();
            var mockStudentsService = new Mock<IUsers>();

            var highScoreId = 1;

            mockHighScoresService.Setup(s => s.DeleteHighScoreAsync(highScoreId)).ThrowsAsync(new Exception());

            var controller = new HighScoresController(mockLeaderboardsService.Object, mockHighScoresService.Object, mockStudentsService.Object);

            // Act
            var result = await controller.DeleteHighScoreById(highScoreId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task Post_AddHighScoreToLeaderboard_ReturnsBadRequest_WhenHighScoreIsNull()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var mockHighScoresService = new Mock<IHighScores>();
            var mockStudentsService = new Mock<IUsers>();

            HighScore highScore = null;
            var leaderboardName = "TestLeaderboard";

            var controller = new HighScoresController(mockLeaderboardsService.Object, mockHighScoresService.Object, mockStudentsService.Object);

            // Act
            var result = await controller.AddHighScoreToLeaderboard(highScore, leaderboardName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_AddHighScoreToLeaderboard_ReturnsBadRequest_WhenLeaderboardNameIsEmpty()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var mockHighScoresService = new Mock<IHighScores>();
            var mockStudentsService = new Mock<IUsers>();

            var highScore = new HighScore { Student = new User { Id = 1 }, Score = 50 };
            string leaderboardName = "";

            var controller = new HighScoresController(mockLeaderboardsService.Object, mockHighScoresService.Object, mockStudentsService.Object);

            // Act
            var result = await controller.AddHighScoreToLeaderboard(highScore, leaderboardName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
*/