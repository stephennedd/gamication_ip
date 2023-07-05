
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using GamificationAPI.Context;
using GamificationAPI.Seed;
using GamificationAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;

using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using GamificationAPI.Services;

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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Bearer Token (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddTransient<ILeaderboards, LeaderboardService>();
builder.Services.AddTransient<IUsers, UserService>();
builder.Services.AddTransient<IHighScores, HighScoreService>();
builder.Services.AddTransient<IGeneratedTests, GeneratedTestService>();
builder.Services.AddTransient<ITests, TestService>();
builder.Services.AddTransient<IBadges, BadgeService>();
builder.Services.AddTransient<IStudentAnswers, StudentAnswerService>();
builder.Services.AddTransient<IStudentQuestions, StudentQuestionService>();
builder.Services.AddTransient<IEmails, EmailService>();
builder.Services.AddTransient<ISubjects, SubjectService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsVerified", policy =>
    {
        policy.RequireClaim("IsVerified", "True");
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

    });


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
                if (service.IsDatabaseEmpty())
                {
                    await service.Seed();
                }       
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if(context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
    SeedData(app);
}
app.Run();