using GamificationAPI.Interfaces;
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
}

public class Subject
{
    public int Id { get; set; }
    public string SubjectTitle { get; set; }
    public int WeekNumber { get; set; }
    public int TestId { get; set; }
    public Test Test { get; set; }
}
