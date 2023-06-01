using BulkyBookWeb.Models;
using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface IGeneratedTests
    {
        Task<List<GeneratedTest>> GetGeneratedTests();
        Task<GeneratedTest> GetGeneratedTestById(int id);
        Task<GeneratedTest> GenerateTest(int studentId, int testId, int numberOfQuestions);
    }
}
