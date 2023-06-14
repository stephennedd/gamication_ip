
using BulkyBookWeb.Models;
using GamificationAPI.Models;
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

                foreach (var role in gamificationToIpData.roles)
                {
                    var newRole = new Role
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                    applicationDbContext.Set<Role>().Add(newRole);
                    await applicationDbContext.SaveChangesAsync();
                }

                foreach (var group in gamificationToIpData.groups)
                {
                    var newGroup = new Group
                    {
                        Id = group.Id,
                        Name = group.Name
                    };
                    applicationDbContext.Set<Group>().Add(newGroup);
                    await applicationDbContext.SaveChangesAsync();
                }

                foreach (var user in gamificationToIpData.users)
                {
                    int roleId = user.RoleId;
                    Role role = applicationDbContext.Set<Role>().Find(roleId);
                    int? groupId = user.GroupId;
                    var newUser = new User
                    {
                        UserId = user.UserId,
                        Password = user.Password,
                        Role = role
                    };
                    if (groupId != null)
                    { 
                        Group group = applicationDbContext.Set<Group>().Find(groupId);

                        newUser.Group = group;
                            
                    }
                    

                    applicationDbContext.Set<User>().Add(newUser);
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

                foreach (var subject in gamificationToIpData.subjects)
                {
                    var newSubject = new Subject
                    {
                        SubjectTitle= subject.SubjectTitle,
                        WeekNumber= subject.WeekNumber,
                        TestId= subject.TestId,
                    };

                    applicationDbContext.Set<Subject>().Add(newSubject);
                    await applicationDbContext.SaveChangesAsync();
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