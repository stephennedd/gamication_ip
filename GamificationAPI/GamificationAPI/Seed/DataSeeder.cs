using BulkyBookWeb.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;



namespace GamificationToIP.Seed
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(ApplicationDbContext applicationDbContext, ILogger<DatabaseSeeder> logger)
        {
            this.applicationDbContext = applicationDbContext;
            _logger = logger;
        }
        public async Task Seed()
        {
            await ClearData(); // Clear existing data before seeding

            using (StreamReader reader = new StreamReader("Seed\\gamificationToIpData.json"))
            {
                string jsonData = await reader.ReadToEndAsync();
                dynamic gamificationToIpData = JsonConvert.DeserializeObject(jsonData);

                _logger.LogInformation("This is an information message.");

                foreach (var student in gamificationToIpData.students)
                {
                    var newStudent = new Student
                    {
                     FirstName = student?.FirstName,
                     LastName = student?.LastName,
                     MiddleName = student?.MiddleName,
                     Email = student?.Email,
                     Password = student?.Password,
                     IsBanned = student?.IsBanned,     
                    };

                    applicationDbContext.Set<Student>().Add(newStudent);
                    await applicationDbContext.SaveChangesAsync();
                }

                    foreach (var test in gamificationToIpData.tests)
                {
                    var newTest = new Test
                    {
                        Title = test?.title,
                        ImageUrl = test?.image_url,
                        Description = test?.description,
                        TimeSeconds = test?.time_seconds
                    };

                    applicationDbContext.Set<Test>().Add(newTest);
                    await applicationDbContext.SaveChangesAsync();

                    foreach (var question in test?.questions)
                    {
                        var newQuestion = new Question
                        {
                            QuestionText = question.question,
                            CorrectAnswer = question.correct_answer,
                            SelectedAnswer= question.selected_answer,
                            TestId = newTest.Id
                        };

                        applicationDbContext.Set<Question>().Add(newQuestion);
                        await applicationDbContext.SaveChangesAsync();

                        foreach (var answer in question.answers)
                        {
                            var newAnswer = new Answer
                            {
                                Identifier = answer.identifier,
                                AnswerText = answer.answer,
                                QuestionId = newQuestion.Id
                     
                            };

                            applicationDbContext.Set<Answer>().Add(newAnswer);
                            await applicationDbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private async Task ClearData()
        {
            var answers = applicationDbContext.Answers.ToList();
            var questions = applicationDbContext.Questions.ToList();
            var tests = applicationDbContext.Tests.ToList();

            applicationDbContext.Answers.RemoveRange(answers);
            applicationDbContext.Questions.RemoveRange(questions);
            applicationDbContext.Tests.RemoveRange(tests);

            await applicationDbContext.SaveChangesAsync();
        }
    }

}