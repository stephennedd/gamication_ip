using BulkyBookWeb.Models;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;

namespace GamificationToIP.Context
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<StudentResult> StudentResults { get; set; }

    }
}
