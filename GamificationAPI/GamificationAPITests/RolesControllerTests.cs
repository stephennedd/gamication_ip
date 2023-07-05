using GamificationAPI.Context;
using GamificationAPI.Controllers;
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationAPI.Context;
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
        private RolesController _controller;
        private ApplicationDbContext _dbContext;

        public RolesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique name for in-memory database to avoid clashing keys
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _controller = new RolesController(_dbContext);

            SeedDatabase(); // Call method to seed the database
        }

        private void SeedDatabase()
        {
            _dbContext.Roles.Add(new Role { Id = 1, Name = "Admin" });
            _dbContext.Roles.Add(new Role { Id = 2, Name = "Teacher" });
            _dbContext.Roles.Add(new Role { Id = 3, Name = "Student" });
            _dbContext.SaveChanges();
        }

        [Fact]
        public void GetRoles_ReturnsOkResult_WithListOfRoles()
        {
            // Act
            var result = _controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsType<List<Role>>(okResult.Value);
            Assert.Equal(3, roles.Count);
        }

    }

}
