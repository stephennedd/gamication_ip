
using BulkyBookWeb.Models;
using GamificationAPI.Models;
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
        public DbSet<HighScore> HighScores { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }
        public DbSet<GeneratedTest> GeneratedTest { get; set; }
        public DbSet<StudentQuestion> StudentQuestions { get; set; }

    }
}
