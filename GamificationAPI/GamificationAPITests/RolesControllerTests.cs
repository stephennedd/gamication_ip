using Xunit;
using GamificationAPI.Controllers;
using GamificationToIP.Context;
using Microsoft.EntityFrameworkCore;
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class RolesControllerTests
{
    private RolesController _controller;
    private ApplicationDbContext _dbContext;

    public RolesControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

        _dbContext = new ApplicationDbContext(options);
        _controller = new RolesController(_dbContext);
    }
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ApplicationDbContext(options);
        return dbContext;

    }
        [Fact]
    public void AddRole_ReturnsOkResult_WhenRoleIsValid()
    {
        // Act
        var result = _controller.AddRole("TestRole");

        // Assert
        Assert.IsType<OkResult>(result);
        var role = _dbContext.Roles.SingleOrDefault(r => r.Name == "TestRole");
        Assert.NotNull(role);
    }

    [Fact]
    public void AddRole_ReturnsBadRequest_WhenRoleIsNull()
    {
        // Act
        var result = _controller.AddRole(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void GetRoles_ReturnsEmptyList_WhenNoRolesExist()
    {
        // Act
        var okResult = _controller.GetRoles() as OkObjectResult;

        // Assert
        Assert.NotNull(okResult);
        var items = Assert.IsType<List<Role>>(okResult.Value);
        Assert.Empty(items);
    }

    [Fact]
    public void AddRole_CreatesNewRoleInDb()
    {
        // Act
        var result = _controller.AddRole("NewRole");

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Single(_dbContext.Roles);
        Assert.Equal("NewRole", _dbContext.Roles.Single().Name);
    }

    [Fact]
    public void DeleteRole_ReturnsBadRequest_WhenIdIsZero()
    {
        // Act
        var result = _controller.DeleteRole(0);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void DeleteRole_RemovesRoleFromDb()
    {
        // Arrange
        _dbContext.Roles.Add(new Role { Name = "TestRole" });
        _dbContext.SaveChanges();
        var initialRoleCount = _dbContext.Roles.Count();

        // Act
        var result = _controller.DeleteRole(1);

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Equal(initialRoleCount - 1, _dbContext.Roles.Count());
    }

    [Fact]
    public void DeleteRole_ReturnsOkResult_WhenRoleExists()
    {
        // Arrange
        var role = new Role { Name = "TestRole" };
        _dbContext.Roles.Add(role);
        _dbContext.SaveChanges();

        // Act
        var result = _controller.DeleteRole(role.Id);

        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Null(_dbContext.Roles.Find(role.Id));
    }

    [Fact]
    public void DeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        // Act
        var result = _controller.DeleteRole(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    [Fact]
    public void AddRole_ReturnsBadRequest_WhenRoleNameIsNull()
    {
        // Act
        var result = _controller.AddRole(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void AddRole_ReturnsBadRequest_WhenRoleNameIsEmpty()
    {
        // Act
        var result = _controller.AddRole("");

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }


    [Fact]
    public void GetRoles_ReturnsCorrectNumberOfRoles()
    {
        // Arrange
        _dbContext.Roles.Add(new Role { Name = "Role1" });
        _dbContext.Roles.Add(new Role { Name = "Role2" });
        _dbContext.SaveChanges();

        // Act
        var okResult = _controller.GetRoles() as OkObjectResult;

        // Assert
        Assert.NotNull(okResult);
        var items = Assert.IsType<List<Role>>(okResult.Value);
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void GetRoles_ReturnsCorrectRoleNames()
    {
        // Arrange
        _dbContext.Roles.Add(new Role { Name = "Role1" });
        _dbContext.Roles.Add(new Role { Name = "Role2" });
        _dbContext.SaveChanges();

        // Act
        var okResult = _controller.GetRoles() as OkObjectResult;

        // Assert
        Assert.NotNull(okResult);
        var items = Assert.IsType<List<Role>>(okResult.Value);
        Assert.Contains(items, r => r.Name == "Role1");
        Assert.Contains(items, r => r.Name == "Role2");
    }
    private void ResetDatabase()
    {
        // Dispose of the previous context and create a new one.
        _dbContext?.Dispose();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
    }
}
