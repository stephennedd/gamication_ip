using GamificationAPI.Controllers;
using GamificationAPI.Models;
using GamificationToIP.Context;
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
    public class RolesControllerTests
    {
        private readonly RolesController _controller;
        private readonly Mock<DbSet<Role>> _mockSet;

        public RolesControllerTests()
        {
            var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Role1" },
            new Role { Id = 2, Name = "Role2" },
            new Role { Id = 3, Name = "Role3" }
        }.AsQueryable();

            _mockSet = new Mock<DbSet<Role>>();
            _mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.Provider);
            _mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.Expression);
            _mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.ElementType);
            _mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Roles).Returns(_mockSet.Object);

            _controller = new RolesController(mockContext.Object);
        }

        [Fact]
        public void GetRoles_ReturnsAllRoles()
        {
            var result = _controller.GetRoles();

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var roles = okResult.Value as List<Role>;

            Assert.Equal(3, roles.Count);
        }

        [Fact]
        public void DeleteRole_ReturnsOkResult_WhenRoleExists()
        {
            var result = _controller.DeleteRole(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            var result = _controller.DeleteRole(0);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
public void AddRole_ReturnsBadRequest_WhenRoleNameIsNullOrWhitespace()
{
    // Arrange
    string roleName = null; // or string.Empty or "   ";

    // Act
    var result = _controller.AddRole(roleName);

    // Assert
    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Role name cannot be null or empty", badRequestResult.Value);
}

    }
}
