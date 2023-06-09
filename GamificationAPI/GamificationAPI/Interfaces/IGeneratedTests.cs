using BulkyBookWeb.Models;

using GamificationAPI.Models;

using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Interfaces

{

    public interface IGeneratedTests

    {

        Task<List<GeneratedTest>> GetGeneratedTests();

        Task<GeneratedTest> GetGeneratedTestById(int id);

        Task<GeneratedTest> GenerateTest(int studentId, int testId, int numberOfQuestions);

        Task<GeneratedTestDto> GetGeneratedTest(int studentId, int testId);

        Task<ActionResult<string>> SaveStudentAnswer(int studentQuestionId, int answerId);

    }

}
