
using BulkyBookWeb.Models;
using GamificationAPI.Models;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace GamificationToIP.Context
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<StudentResult> StudentResults { get; set; }
        public DbSet<HighScore> HighScores { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }
        public DbSet<GeneratedTest> GeneratedTest { get; set; }
        public DbSet<StudentQuestion> StudentQuestions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Badge> Badges { get; set; }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.HighScores)
                .WithOne(e => e.User)            
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasMany(b => b.Badges)
                .WithMany(u => u.Users);
            
            modelBuilder.Entity<Leaderboard>()
                .HasMany(e => e.HighScores)
                .WithOne(e => e.Leaderboard)
                .IsRequired(true);
        }
    }
}
