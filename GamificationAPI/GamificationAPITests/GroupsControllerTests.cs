using GamificationAPI.Controllers;
using GamificationAPI.Models;
using GamificationToIP.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class GroupsControllerTests
{
    private GroupsController _controller;
    private ApplicationDbContext _dbContext;

    public GroupsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

        _dbContext = new ApplicationDbContext(options);
        _controller = new GroupsController(_dbContext);
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
    public void AddGroup_ReturnsOkResult_WhenGroupNameIsValid()
    {
        // Act
        var result = _controller.AddGroup("ValidGroupName");

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetGroups_ReturnsOkResult_WithAListOfGroups()
    {
        // Arrange
        _dbContext.Groups.Add(new Group { Name = "Group1", Id = 1 });
        _dbContext.Groups.Add(new Group { Name = "Group2", Id = 2 });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = _controller.GetGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var groups = Assert.IsType<List<Group>>(okResult.Value);
        Assert.Equal(2, groups.Count);
    }

    [Fact]
    public async Task DeleteGroup_ReturnsOkResult_WhenGroupIdIsValid()
    {
        // Arrange
        var group = new Group { Name = "Group1", Id = 1 };
        _dbContext.Groups.Add(group);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = _controller.DeleteGroup(1);

        // Assert
        Assert.IsType<OkResult>(result);
    }


    [Fact]
    public void DeleteGroup_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var mockContext = GetInMemoryDbContext();
        var controller = new GroupsController(mockContext);

        // Act
        var result = controller.DeleteGroup(999);  // Non-existent Id

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void DeleteGroup_ReturnsOk_WhenGroupExists()
    {
        // Arrange
        var mockContext = GetInMemoryDbContext();

        var group = new Group { Name = "TestGroup", Id = 1 };
        mockContext.Groups.Add(group);
        mockContext.SaveChanges();

        var controller = new GroupsController(mockContext);

        // Act
        var result = controller.DeleteGroup(1);  // Existing Id

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void AddGroup_ReturnsBadRequest_WhenGroupNameIsNull()
    {
        // Arrange
        var mockContext = GetInMemoryDbContext();
        var controller = new GroupsController(mockContext);

        // Act
        var result = controller.AddGroup(null);  // Null GroupName

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public void AddGroup_ReturnsOk_WhenGroupNameIsValid()
    {
        // Arrange
        var mockContext = GetInMemoryDbContext();
        var controller = new GroupsController(mockContext);

        // Act
        var result = controller.AddGroup("TestGroup");  // Valid GroupName

        // Assert
        Assert.IsType<OkResult>(result);
    }
}