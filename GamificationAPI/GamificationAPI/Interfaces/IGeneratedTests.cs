
using GamificationAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Interfaces
{
    public interface IGeneratedTests
    {
        Task<List<GeneratedTest>> GetGeneratedTests();
        Task<GeneratedTest> GetGeneratedTestById(int id);
        Task<GeneratedTest> GenerateTest(string studentId, int testId, int numberOfQuestions);
        //Task<GeneratedTestDto> GetGeneratedTest(string studentId, int testId);
        Task<ActionResult<string>> SaveStudentAnswer(int studentQuestionId, int answerId);
    }
}
