
using GamificationAPI.Interfaces;
using GamificationAPI.Models;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;
using System.Threading.Tasks;



public class SubjectService : ISubjects
{
    private readonly ApplicationDbContext _dbContext;

    public SubjectService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<Subject>> GetSubjects()
    {
        var subjects = await _dbContext.Subjects
            .Include(s => s.Test)
                .ThenInclude(t => t.Questions)
                    .ThenInclude(q => q.Answers)
                    .ToListAsync();

        if (subjects == null)
        {
            throw new NotFoundException();
        }

        return subjects;
    }

    public async Task<Subject> DeleteSubject(int id)
    {
        Subject subject = await _dbContext.Subjects.FindAsync(id);
       

        // Check if the subject has an associated test
        if (subject.TestId != null)
        {
            Test test = await _dbContext.Tests
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == subject.TestId);

            if (test != null)
            {
                // Remove the questions and answers of the test
                _dbContext.Answers.RemoveRange(test.Questions.SelectMany(q => q.Answers));
                _dbContext.Questions.RemoveRange(test.Questions);

                _dbContext.Tests.Remove(test);
            }
        }

        _dbContext.Subjects.Remove(subject);
        await _dbContext.SaveChangesAsync();

        return subject;
    }

    public async Task<Subject> AddSubject(NewSubject newSubject)
    {
        var newTest = new Test
        {
            Title = newSubject.SubjectTitle,
            ImageUrl = "",
            Description = "",
            TimeSeconds = 0
        };

        _dbContext.Set<Test>().Add(newTest);
        await _dbContext.SaveChangesAsync();

        int newTestId = newTest.Id;

        var subject = new Subject
        {
            SubjectTitle = newSubject.SubjectTitle,
            WeekNumber = newSubject.WeekNumber,
            TestId = newTestId,
            GameId = newSubject.GameId
        };

        // Add the subject to the context
        _dbContext.Subjects.Add(subject);


        await _dbContext.SaveChangesAsync();


        return subject;
    }
    public async Task<RootObject> UpdateTables(RootObject data)
    {

        // Remove the existing questions not provided in the RootObject
        var existingQuestions = _dbContext.Questions.Where(q => q.TestId == data.Test.Id).ToList();
        foreach (var question in existingQuestions)
        {
            var providedQuestion = data.Test.Questions.FirstOrDefault(q => q.Id == question.Id);
            if (providedQuestion == null)
                _dbContext.Questions.Remove(question);
        }

        // Update the Subject record
        var existingSubject = _dbContext.Subjects.FirstOrDefault(s => s.Id == data.Id);
        if (existingSubject != null)
        {
            existingSubject.SubjectTitle = data.SubjectTitle;
            existingSubject.WeekNumber = data.WeekNumber;
            existingSubject.TestId = data.TestId;
        }

        // Update the Test record
        var existingTest = _dbContext.Tests.FirstOrDefault(t => t.Id == data.Test.Id);
        if (existingTest != null)
        {
            existingTest.Title = data.Test.Title;
            existingTest.ImageUrl = data.Test.ImageUrl;
            existingTest.Description = data.Test.Description;
            existingTest.TimeSeconds = data.Test.TimeSeconds;
        }

        // Update or add the Questions and Answers records
        foreach (var question in data.Test.Questions)
        {
            var existingQuestion = _dbContext.Questions.FirstOrDefault(q => q.Id == question.Id);
            if (existingQuestion != null)
            {
                existingQuestion.QuestionText = question.QuestionText;
                existingQuestion.CorrectAnswer = question.CorrectAnswer;
                existingQuestion.SelectedAnswer = question.SelectedAnswer;
                existingQuestion.TestId = question.TestId;

                // Remove the existing answers not provided in the RootObject
                var existingAnswers = _dbContext.Answers.Where(a => a.QuestionId == existingQuestion.Id).ToList();
                foreach (var answer in existingAnswers)
                {
                    var providedAnswer = question.Answers.FirstOrDefault(a => a.Id == answer.Id);
                    if (providedAnswer == null)
                        _dbContext.Answers.Remove(answer);
                }
            }
            else
            {
                existingQuestion = new Question
                {
                    QuestionText = question.QuestionText,
                    CorrectAnswer = question.CorrectAnswer,
                    SelectedAnswer = question.SelectedAnswer,
                    TestId = question.TestId
                };
                _dbContext.Questions.Add(existingQuestion);
                _dbContext.SaveChanges();
            }

            foreach (var answer in question.Answers)
            {
                var existingAnswer = _dbContext.Answers.FirstOrDefault(a => a.Id == answer.Id);
                if (existingAnswer != null)
                {
                    existingAnswer.Identifier = answer.Identifier;
                    existingAnswer.AnswerText = answer.AnswerText;
                    existingAnswer.QuestionId = answer.QuestionId;
                }
                else
                {
                    existingAnswer = new Answer
                    {
                        Identifier = answer.Identifier,
                        AnswerText = answer.AnswerText,
                        QuestionId = existingQuestion.Id
                    };
                    _dbContext.Answers.Add(existingAnswer);
                }
            }
        }

        // Save changes to the database
        _dbContext.SaveChanges();
        // Return the updated RootObject
        var updatedRootObject = new RootObject
        {
            Id = existingSubject?.Id ?? 0,
            SubjectTitle = existingSubject?.SubjectTitle,
            WeekNumber = existingSubject?.WeekNumber ?? 0,
            TestId = existingSubject?.TestId ?? 0,
            Test = new SubjectTest
            {
                Id = existingTest?.Id ?? 0,
                Title = existingTest?.Title,
                ImageUrl = existingTest?.ImageUrl,
                Description = existingTest?.Description,
                TimeSeconds = existingTest?.TimeSeconds ?? 0,
                Questions = _dbContext.Questions
                    .Where(q => q.TestId == existingTest.Id)
                    .Select(q => new SubjectQuestion
                    {
                        Id = q.Id,
                        QuestionText = q.QuestionText,
                        CorrectAnswer = q.CorrectAnswer,
                        SelectedAnswer = q.SelectedAnswer,
                        TestId = q.TestId,
                        Answers = _dbContext.Answers
                            .Where(a => a.QuestionId == q.Id)
                            .Select(a => new SubjectAnswer
                            {
                                Id = a.Id,
                                Identifier = a.Identifier,
                                AnswerText = a.AnswerText,
                                QuestionId = a.QuestionId
                            })
                            .ToList()
                    })
                    .ToList()
            }
        };

        return updatedRootObject;
    }

}
