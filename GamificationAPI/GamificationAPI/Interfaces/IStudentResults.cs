
using GamificationAPI.Models;
using GamificationToIP.Models;

namespace GamificationAPI.Interfaces
{
    public interface IStudentResults
    {
        Task<List<StudentResult>> GetStudentResultsAsync();
        Task CreateStudentResultAsync(int testId, int studentId);
    }
}
