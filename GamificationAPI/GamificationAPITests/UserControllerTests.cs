using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Controllers;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class UserControllerTests
{
    private readonly Mock<IUsers> _userServiceMock;
    private readonly UsersController _userController;
    private readonly User _testUser;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUsers>();
        _userController = new UsersController(null, _userServiceMock.Object);

        _testUser = new User
        {
            Id = "testId",
            Password = "testPassword",
            IsVerified = false
        };
    }

    // Edge case where ID does not exist for GET request
    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _userServiceMock.Setup(x => x.UserExistsAsync(_testUser.Id)).ReturnsAsync(false);

        var result = await _userController.GetUser(_testUser.Id);

        Assert.IsType<NotFoundResult>(result);
    }

    // Edge case where user exists but is null for GET request
    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserIsNull()
    {
        _userServiceMock.Setup(x => x.UserExistsAsync(_testUser.Id)).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.GetUserByIdAsync(_testUser.Id)).ReturnsAsync((User)null);

        var result = await _userController.GetUser(_testUser.Id);

        Assert.IsType<NotFoundResult>(result);
    }

    

    

    // Test PUT operation with mismatched user ID
    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenIdDoesNotMatchUser()
    {
        var result = await _userController.UpdateUser("wrongId", _testUser);

        Assert.IsType<BadRequestResult>(result);
    }

    // Test DELETE operation with invalid user ID
    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _userServiceMock.Setup(x => x.GetUserByIdAsync(_testUser.Id)).ReturnsAsync((User)null);

        var result = await _userController.DeleteUser(_testUser.Id);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // Test CREATE operation with existing user ID
    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenUserIdExists()
    {
        UserCredentials userCredentials = new UserCredentials { Id = _testUser.Id, Password = _testUser.Password };

        _userServiceMock.Setup(x => x.UserExistsAsync(userCredentials.Id)).ReturnsAsync(true);

        var result = await _userController.CreateUser(userCredentials);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}