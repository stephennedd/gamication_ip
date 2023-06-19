﻿using GamificationAPI.Interfaces;
using GamificationToIP.Context;
using GamificationToIP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;


[Route("api/subjects")]
[ApiController]
public class SubjectController : ControllerBase
{

    private readonly ISubjects _subjectService;
    private readonly ApplicationDbContext _dbContext;

    public SubjectController(ISubjects subjectService, ApplicationDbContext dbContext)
    {
        _subjectService = subjectService;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Subject>>> GetSubjects()
    {
        var subjects = await _subjectService.GetSubjects();

        if (subjects == null)
        {
            return NotFound();
        }

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(subjects, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }
    
    [HttpPost]
    public async Task<ActionResult<Subject>> AddSubject([FromBody] NewSubject newSubject)
    {
        var subject = await _subjectService.AddSubject(newSubject);
        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        var json = JsonConvert.SerializeObject(subject, Formatting.None, jsonSettings);
        return Content(json, "application/json");
    }

    [HttpPut]
    public async Task<ActionResult<RootObject>> UpdateTables(RootObject data)
    {
        var updatedRootObject = await _subjectService.UpdateTables(data);
        
        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        var json = JsonConvert.SerializeObject(updatedRootObject, Formatting.None, jsonSettings);

        return Content(json, "application/json");
    }
}

public class NewSubject
{
    public string SubjectTitle { get; set; }
    public int WeekNumber { get; set; }
    public int GameId { get; set; }
}
public class Subject
{
    public int Id { get; set; }
    public string SubjectTitle { get; set; }
    public int WeekNumber { get; set; }
    public int TestId { get; set; }
    public Test Test { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; }
}

public class RootObject
{
    public int Id { get; set; }
    public string SubjectTitle { get; set; }
    public int WeekNumber { get; set; }
    public int TestId { get; set; }
    public SubjectTest Test { get; set; }
}

public class SubjectTest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public int TimeSeconds { get; set; }
    public List<SubjectQuestion> Questions { get; set; }
}

public class SubjectQuestion
{
    public int? Id { get; set; }
    public string QuestionText { get; set; }
    public string CorrectAnswer { get; set; }
    public string SelectedAnswer { get; set; }
    public int TestId { get; set; }
    public List<SubjectAnswer> Answers { get; set; }
}

public class SubjectAnswer
{
    public int?  Id { get; set; }
    public string Identifier { get; set; }
    public string AnswerText { get; set; }
    public int QuestionId { get; set; }
}

