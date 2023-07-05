using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamificationAPITests
{
    public class LeaderboardsControllerTest
    {
        private Mock<ILeaderboards> _mockLeaderboardsService;
        private Mock<IUsers> _mockUserService;
        private LeaderboardsController _controller;
        private ApplicationDbContext _context;

        public LeaderboardsControllerTest()
        {
            // Setup DbContext with InMemory
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            // Setup Mock Services
            _mockLeaderboardsService = new Mock<ILeaderboards>();
            _mockUserService = new Mock<IUsers>();

            // Instantiate the controller
            _controller = new LeaderboardsController(_mockLeaderboardsService.Object, _mockUserService.Object, _context);
        }

        [Fact]
        public async Task GetAllLeaderboard_ShouldReturnNoContent_WhenNoLeaderboardsExist()
        {
            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardsSimpleAsync())
                .ReturnsAsync(new List<Leaderboard>());

            var result = await _controller.GetAllLeaderboard();

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAllLeaderboard_ShouldReturnOk_WhenLeaderboardsExist()
        {
            var leaderboards = new List<Leaderboard>
        {
            new Leaderboard { Name = "Leaderboard1" },
            new Leaderboard { Name = "Leaderboard2" },
        };

            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardsSimpleAsync())
                .ReturnsAsync(leaderboards);

            var result = await _controller.GetAllLeaderboard();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Leaderboard>>(okResult.Value);
            Assert.Equal(leaderboards.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetLeaderboardById_ShouldReturnBadRequest_WhenLeaderboardNameIsEmpty()
        {
            var result = await _controller.GetLeaderboardById("", null);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetLeaderboardById_ShouldReturnNotFound_WhenLeaderboardDoesNotExist()
        {
            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Leaderboard)null);

            var result = await _controller.GetLeaderboardById("non-existent-leaderboard", null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetLeaderboardById_ShouldReturnOk_WhenLeaderboardExists()
        {
            var leaderboard = new Leaderboard { Name = "Leaderboard1" };

            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(leaderboard);

            var result = await _controller.GetLeaderboardById("Leaderboard1", null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Leaderboard>(okResult.Value);
            Assert.Equal(leaderboard.Name, returnValue.Name);
        }
        [Fact]
        public async Task GetLeaderboardById_ShouldReturnLeaderboardForGroup_WhenGroupNameProvided()
        {
            // Set up a leaderboard with high scores for a specific group
            var group = new Group { Name = "Test Group" };
            var leaderboard = new Leaderboard
            {
                Name = "Leaderboard1",
                HighScores = new List<HighScore>
        {
            new HighScore
            {
                User = new User { Group = group },
                Score = 10
            },
            new HighScore
            {
                User = new User { Group = new Group { Name = "Other Group" } },
                Score = 20
            }
        }
            };

            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(leaderboard);
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Request leaderboard for "Test Group"
            var result = await _controller.GetLeaderboardById("Leaderboard1", "Test Group");

            // Verify that the returned leaderboard only includes scores for "Test Group"
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Leaderboard>(okResult.Value);
            Assert.Single(returnValue.HighScores);
            Assert.Equal(group.Name, returnValue.HighScores.First().User.Group.Name);
        }

        [Fact]
        public async Task UpdateLeaderboard_ShouldReturnBadRequest_WhenNewLeaderboardNameIsEmpty()
        {
            var result = await _controller.UpdateLeaderboard("Leaderboard1", "");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateLeaderboard_ShouldReturnNotFound_WhenLeaderboardDoesNotExist()
        {
            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Leaderboard)null);

            var result = await _controller.UpdateLeaderboard("non-existent-leaderboard", "NewName");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateLeaderboard_ShouldReturnOk_WhenLeaderboardNameIsUpdated()
        {
            var leaderboard = new Leaderboard { Name = "Leaderboard1" };

            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(leaderboard);
            _mockLeaderboardsService.Setup(svc => svc.UpdateLeaderboardAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await _controller.UpdateLeaderboard("Leaderboard1", "NewName");

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateLeaderboard_ShouldReturnBadRequest_WhenUpdateFails()
        {
            var leaderboard = new Leaderboard { Name = "Leaderboard1" };

            _mockLeaderboardsService.Setup(svc => svc.GetLeaderboardByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(leaderboard);
            _mockLeaderboardsService.Setup(svc => svc.UpdateLeaderboardAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _controller.UpdateLeaderboard("Leaderboard1", "NewName");

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

}
