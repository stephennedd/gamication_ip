using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;

namespace GamificationAPITests
{
    public class LeaderboardControllerTests
    {
        [Fact]
        public async Task Get_GetAllLeaderboard_ReturnsOkResult_WithAListOfLeaderboards()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var controller = new LeaderboardController(mockLeaderboardsService.Object);

            // Act
            var result = await controller.GetAllLeaderboard();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var leaderboards = Assert.IsType<List<Leaderboard>>(okResult.Value);
        }

        [Fact]
        public async Task Get_GetLeaderboardById_ReturnsNotFound_WhenLeaderboardDoesNotExist()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var controller = new LeaderboardController(mockLeaderboardsService.Object);

            var leaderboardName = "NonExistentLeaderboard";

            mockLeaderboardsService.Setup(s => s.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync((Leaderboard)null);

            // Act
            var result = await controller.GetLeaderboardById(leaderboardName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_CreateNewLeaderboard_ReturnsOK_WhenLeaderboardIsCreated()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var controller = new LeaderboardController(mockLeaderboardsService.Object);

            var leaderboardName = "NewLeaderboard";

            mockLeaderboardsService.Setup(s => s.CreateLeaderboardAsync(leaderboardName)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.CreateNewLeaderboard(leaderboardName);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_DeleteLeaderboard_ReturnsOK_WhenLeaderboardIsDeleted()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var controller = new LeaderboardController(mockLeaderboardsService.Object);

            var leaderboardName = "LeaderboardToDelete";

            mockLeaderboardsService.Setup(s => s.DeleteLeaderboardAsync(leaderboardName)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.DeleteLeaderboard(leaderboardName);

            // Assert
            Assert.IsType<OkResult>(result);
        }
        [Fact]
        public async Task Get_GetLeaderboardById_ReturnsBadRequest_WhenLeaderboardNameIsEmpty()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var controller = new LeaderboardController(mockLeaderboardsService.Object);

            string leaderboardName = "";

            // Act
            var result = await controller.GetLeaderboardById(leaderboardName);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Post_CreateNewLeaderboard_ReturnsBadRequest_WhenLeaderboardNameIsEmpty()
        {
            // Arrange
            var mockLeaderboardsService = new Mock<ILeaderboards>();
            var controller = new LeaderboardController(mockLeaderboardsService.Object);

            string leaderboardName = "";

            // Act
            var result = await controller.CreateNewLeaderboard(leaderboardName);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
    
}