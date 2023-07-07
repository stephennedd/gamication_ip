

using GamificationAPI.Models;
using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface ITests
    {
        Task<List<Test>> GetTestsAsync();
        Task<TestDto> GetTestByIdAsync(int id);
        Task<List<QuestionDto>> GetQuestionsByTestIdAsync(int testId);
        Task<List<Question>> GetQuestionsByIdsAsync(List<int> questionIds);
        Task<TestDto> CreateTest(TestDto test);
        Task<QuestionDto> AddQuestionToTest(int testId, QuestionDto questionDto);
    }
}