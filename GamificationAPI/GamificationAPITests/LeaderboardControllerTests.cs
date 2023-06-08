using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class LeaderboardsControllerTests
{
    private Mock<ILeaderboards> _mockLeaderboardService;
    private LeaderboardsController _controller;

    public LeaderboardsControllerTests()
    {
        _mockLeaderboardService = new Mock<ILeaderboards>();
        _controller = new LeaderboardsController(_mockLeaderboardService.Object);
    }

    [Fact]
    public async Task GetAllLeaderboard_ReturnsOkResult_WithLeaderboards()
    {
        // Arrange
        _mockLeaderboardService.Setup(service => service.GetLeaderboardsAsync()).ReturnsAsync(GetTestLeaderboards());

        // Act
        var result = await _controller.GetAllLeaderboard();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var items = Assert.IsType<List<Leaderboard>>(okResult.Value);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task GetLeaderboardById_ReturnsOkResult_WithLeaderboard()
    {
        // Arrange
        string leaderboardName = "Test Leaderboard";
        _mockLeaderboardService.Setup(service => service.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync(new Leaderboard { Name = leaderboardName });

        // Act
        var result = await _controller.GetLeaderboardById(leaderboardName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var item = Assert.IsType<Leaderboard>(okResult.Value);
        Assert.Equal(leaderboardName, item.Name);
    }

    [Fact]
    public async Task CreateNewLeaderboard_ReturnsOkResult_WhenLeaderboardDoesNotExist()
    {
        // Arrange
        string leaderboardName = "New Leaderboard";
        _mockLeaderboardService.Setup(service => service.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync((Leaderboard)null);

        // Act
        var result = await _controller.CreateNewLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteLeaderboard_ReturnsOkResult_WhenLeaderboardExists()
    {
        // Arrange
        string leaderboardName = "Test Leaderboard";
        _mockLeaderboardService.Setup(service => service.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync(new Leaderboard { Name = leaderboardName });
        _mockLeaderboardService.Setup(service => service.DeleteLeaderboardAsync(leaderboardName)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    [Fact]
    public async Task GetLeaderboardById_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string leaderboardName = null;

        // Act
        var result = await _controller.GetLeaderboardById(leaderboardName);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetLeaderboardById_ReturnsNotFound_WhenLeaderboardDoesNotExist()
    {
        // Arrange
        string leaderboardName = "Non-Existent Leaderboard";
        _mockLeaderboardService.Setup(service => service.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync((Leaderboard)null);

        // Act
        var result = await _controller.GetLeaderboardById(leaderboardName);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateNewLeaderboard_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string leaderboardName = null;

        // Act
        var result = await _controller.CreateNewLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateNewLeaderboard_ReturnsConflict_WhenLeaderboardAlreadyExists()
    {
        // Arrange
        string leaderboardName = "Existing Leaderboard";
        _mockLeaderboardService.Setup(service => service.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync(new Leaderboard { Name = leaderboardName });

        // Act
        var result = await _controller.CreateNewLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task DeleteLeaderboard_ReturnsBadRequest_WhenNameIsNull()
    {
        // Arrange
        string leaderboardName = null;

        // Act
        var result = await _controller.DeleteLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteLeaderboard_ReturnsNotFound_WhenLeaderboardDoesNotExist()
    {
        // Arrange
        string leaderboardName = "Non-Existent Leaderboard";
        _mockLeaderboardService.Setup(service => service.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync((Leaderboard)null);

        // Act
        var result = await _controller.DeleteLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteLeaderboard_ReturnsBadRequest_WhenServiceReturnsFalse()
    {
        // Arrange
        string leaderboardName = "Test Leaderboard";
        _mockLeaderboardService.Setup(service => service.GetLeaderboardByNameAsync(leaderboardName)).ReturnsAsync(new Leaderboard { Name = leaderboardName });
        _mockLeaderboardService.Setup(service => service.DeleteLeaderboardAsync(leaderboardName)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task GetAllLeaderboard_ReturnsNoContent_WhenNoLeaderboardsExist()
    {
        // Arrange
        _mockLeaderboardService.Setup(service => service.GetLeaderboardsAsync()).ReturnsAsync(new List<Leaderboard>());

        // Act
        var result = await _controller.GetAllLeaderboard();

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task CreateNewLeaderboard_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        string leaderboardName = "";

        // Act
        var result = await _controller.CreateNewLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteLeaderboard_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        string leaderboardName = "";

        // Act
        var result = await _controller.DeleteLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreateNewLeaderboard_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        // Arrange
        string leaderboardName = " ";

        // Act
        var result = await _controller.CreateNewLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteLeaderboard_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        // Arrange
        string leaderboardName = " ";

        // Act
        var result = await _controller.DeleteLeaderboard(leaderboardName);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }



private List<Leaderboard> GetTestLeaderboards()
    {
        var leaderboards = new List<Leaderboard>
        {
            new Leaderboard { Name = "Test Leaderboard 1" },
            new Leaderboard { Name = "Test Leaderboard 2" }
        };

        return leaderboards;
    }
}