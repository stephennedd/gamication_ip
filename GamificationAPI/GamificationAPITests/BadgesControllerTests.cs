using System.Collections.Generic;
using System.Threading.Tasks;
using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GamificationAPITests
{
    public class BadgesControllerTests
    {
        private readonly BadgesController _controller;
        private readonly Mock<IBadges> _service;

        public BadgesControllerTests()
        {
            _service = new Mock<IBadges>();

            // Define options for DbContext with UseInMemoryDatabase.
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new ApplicationDbContext(options);
            _controller = new BadgesController(context, _service.Object);
        }

        [Fact]
        public async Task AddBadge_ReturnsOkResult_WhenBadgeIsValid()
        {
            // Arrange
            var badge = new Badge { Name = "Test Badge" };
            _service.Setup(s => s.AddBadgeAsync(badge)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddBadge(badge);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddBadge_ReturnsBadRequestResult_WhenBadgeIsInvalid()
        {
            // Arrange
            var badge = new Badge { Name = "" };
            _service.Setup(s => s.AddBadgeAsync(badge)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddBadge(badge);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetBadges_ReturnsOkResult_WithAListOfBadges()
        {
            // Arrange
            var badges = new List<Badge> { new Badge { Name = "Test Badge" } };
            _service.Setup(s => s.GetBadgesAsync()).ReturnsAsync(badges);

            // Act
            var result = await _controller.GetBadges();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            var actualBadges = okResult.Value as List<Badge>;
            Assert.Equal(badges, actualBadges);
        }
        [Fact]
        public async Task UpdateBadge_ReturnsOkResult_WhenBadgeExists()
        {
            // Arrange
            var badge = new Badge { Id = 1, Name = "Test Badge" };
            _service.Setup(s => s.BadgeExistsAsync(badge.Id)).ReturnsAsync(true);
            _service.Setup(s => s.UpdateBadgeAsync(badge)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateBadge(badge);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateBadge_ReturnsNotFoundResult_WhenBadgeDoesNotExist()
        {
            // Arrange
            var badge = new Badge { Id = 1, Name = "Test Badge" };
            _service.Setup(s => s.BadgeExistsAsync(badge.Id)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBadge(badge);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateBadge_ReturnsBadRequestResult_WhenUpdateFailed()
        {
            // Arrange
            var badge = new Badge { Id = 1, Name = "Test Badge" };
            _service.Setup(s => s.BadgeExistsAsync(badge.Id)).ReturnsAsync(true);
            _service.Setup(s => s.UpdateBadgeAsync(badge)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBadge(badge);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteBadge_ReturnsOkResult_WhenBadgeExists()
        {
            // Arrange
            var id = 1;
            _service.Setup(s => s.BadgeExistsAsync(id)).ReturnsAsync(true);
            _service.Setup(s => s.DeleteBadgeAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBadge(id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteBadge_ReturnsNotFoundResult_WhenBadgeDoesNotExist()
        {
            // Arrange
            var id = 1;
            _service.Setup(s => s.BadgeExistsAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBadge(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBadge_ReturnsBadRequestResult_WhenDeleteFailed()
        {
            // Arrange
            var id = 1;
            _service.Setup(s => s.BadgeExistsAsync(id)).ReturnsAsync(true);
            _service.Setup(s => s.DeleteBadgeAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBadge(id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        [Fact]
        public async Task AddBadge_ReturnsBadRequest_WhenBadgeNameIsEmpty()
        {
            // Arrange
            var badge = new Badge { Name = "" };

            // Act
            var result = await _controller.AddBadge(badge);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateBadge_ReturnsBadRequest_WhenBadgeDoesNotExist()
        {
            // Arrange
            var badge = new Badge { Id = 1, Name = "Test Badge" };
            _service.Setup(s => s.BadgeExistsAsync(badge.Id)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBadge(badge);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBadge_ReturnsNotFound_WhenBadgeDoesNotExist()
        {
            // Arrange
            var id = 1;
            _service.Setup(s => s.BadgeExistsAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBadge(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
        [Fact]
        public async Task AddBadge_ReturnsBadRequest_WhenBadgeIsNull()
        {
            // Arrange
            Badge badge = null;

            // Act
            var result = await _controller.AddBadge(badge);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task AddBadge_ReturnsBadRequest_WhenBadgeAdditionFails()
        {
            // Arrange
            var badge = new Badge { Name = "Test Badge" };
            _service.Setup(s => s.AddBadgeAsync(badge)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddBadge(badge);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateBadge_ReturnsBadRequest_WhenBadgeIsNull()
        {
            // Arrange
            Badge badge = null;

            // Act
            var result = await _controller.UpdateBadge(badge);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateBadge_ReturnsBadRequest_WhenBadgeUpdateFails()
        {
            // Arrange
            var badge = new Badge { Id = 1, Name = "Test Badge" };
            _service.Setup(s => s.BadgeExistsAsync(badge.Id)).ReturnsAsync(true);
            _service.Setup(s => s.UpdateBadgeAsync(badge)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBadge(badge);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteBadge_ReturnsBadRequest_WhenBadgeDeletionFails()
        {
            // Arrange
            var id = 1;
            _service.Setup(s => s.BadgeExistsAsync(id)).ReturnsAsync(true);
            _service.Setup(s => s.DeleteBadgeAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBadge(id);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}