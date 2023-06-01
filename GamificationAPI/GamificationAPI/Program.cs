
using Microsoft.EntityFrameworkCore;
using GamificationToIP.Context;
using GamificationToIP.Seed;
using GamificationAPI.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<DatabaseSeeder>();

// Configure JsonSerializerOptions with ReferenceHandler.Preserve
builder.Services.AddMvc().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

//Register
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ILeaderboards, LeaderboardService>();
builder.Services.AddTransient<IStudents, StudentService>();
builder.Services.AddTransient<IHighScores, HighScoreService>();
builder.Services.AddTransient<IGeneratedTests, GeneratedTestService>();
builder.Services.AddTransient<ITests, TestService>();
builder.Services.AddTransient<IStudentAnswers, StudentAnswerService>();
builder.Services.AddTransient<IStudentQuestions, StudentQuestionService>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();
if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

async void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    if (scopedFactory != null)
    {
        using (var scope = scopedFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<DatabaseSeeder>();
            if (service is not null)
            {
                await service.Seed();
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();