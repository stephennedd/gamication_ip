using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class HighScoresControllerTests
{
    private Mock<ILeaderboards> _leaderboardServiceMock;
    private Mock<IHighScores> _highScoreServiceMock;
    private Mock<IUsers> _userServiceMock;

    public HighScoresControllerTests()
    {
        _leaderboardServiceMock = new Mock<ILeaderboards>();
        _highScoreServiceMock = new Mock<IHighScores>();
        _userServiceMock = new Mock<IUsers>();
    }

    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsBadRequest_WhenHighScoreIsNull()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);

        // Act
        var result = await controller.AddHighScoreToLeaderboard(null, "leaderboardName");

        // Assert
        Assert.IsType<BadRequestResult>(result);

    }
    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsBadRequest_WhenLeaderboardNameIsNull()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);
        var highScore = new HighScore { User = new User { Id = 1.ToString() } };

        // Act
        var result = await controller.AddHighScoreToLeaderboard(highScore, null);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsOk_WhenNewHighScoreIsAdded()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);
        var highScore = new HighScore { User = new User { Id = 1.ToString() } };

        _userServiceMock.Setup(x => x.UserExistsAsync(highScore.User.Id)).ReturnsAsync(true);
        _leaderboardServiceMock.Setup(x => x.CheckIfStudentHasHighScoreInLeadeboard(highScore.User.Id, It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.AddHighScoreToLeaderboard(highScore, "leaderboardName");

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsBadRequest_WhenHighScoreIsNotHighEnough()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);
        var highScore = new HighScore { User = new User { Id = 1.ToString() } };

        _userServiceMock.Setup(x => x.UserExistsAsync(highScore.User.Id)).ReturnsAsync(true);
        _leaderboardServiceMock.Setup(x => x.CheckIfStudentHasHighScoreInLeadeboard(highScore.User.Id, It.IsAny<string>())).ReturnsAsync(true);
        _highScoreServiceMock.Setup(x => x.CheckIfItsHighScore(highScore, It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.AddHighScoreToLeaderboard(highScore, "leaderboardName");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteHighScoreById_ReturnsOk_WhenHighScoreIsDeleted()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);

        // Act
        var result = await controller.DeleteHighScoreById(1);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteHighScoreById_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);

        _highScoreServiceMock.Setup(x => x.DeleteHighScoreAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await controller.DeleteHighScoreById(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);
        controller.ModelState.AddModelError("error", "error");
        var highScore = new HighScore();

        // Act
        var result = await controller.AddHighScoreToLeaderboard(highScore, "leaderboardName");

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);
        var highScore = new HighScore { User = new User { Id = 1.ToString() } };

        _userServiceMock.Setup(x => x.UserExistsAsync(highScore.User.Id)).ReturnsAsync(false);

        // Act
        var result = await controller.AddHighScoreToLeaderboard(highScore, "leaderboardName");

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsOk_WhenHighScoreIsUpdated()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);
        var highScore = new HighScore { User = new User { Id = 1.ToString() } };

        _userServiceMock.Setup(x => x.UserExistsAsync(highScore.User.Id)).ReturnsAsync(true);
        _leaderboardServiceMock.Setup(x => x.CheckIfStudentHasHighScoreInLeadeboard(highScore.User.Id, It.IsAny<string>())).ReturnsAsync(true);
        _highScoreServiceMock.Setup(x => x.CheckIfItsHighScore(highScore, It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await controller.AddHighScoreToLeaderboard(highScore, "leaderboardName");

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteHighScoreById_ReturnsBadRequest_WhenHighScoreIdIsInvalid()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);

        _highScoreServiceMock.Setup(x => x.DeleteHighScoreAsync(-1)).ThrowsAsync(new Exception());

        // Act
        var result = await controller.DeleteHighScoreById(-1);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
    [Fact]
    public async Task AddHighScoreToLeaderboard_ReturnsBadRequest_WhenLeaderboardNameIsEmpty()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);
        var highScore = new HighScore();

        // Act
        var result = await controller.AddHighScoreToLeaderboard(highScore, "");

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }


    [Fact]
    public async Task DeleteHighScoreById_ReturnsOk_WhenDeletionIsSuccessful()
    {
        // Arrange
        var controller = new HighScoresController(_leaderboardServiceMock.Object, _highScoreServiceMock.Object, _userServiceMock.Object);

        _highScoreServiceMock.Setup(x => x.DeleteHighScoreAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteHighScoreById(1);

        // Assert
        Assert.IsType<OkResult>(result);
    }
}