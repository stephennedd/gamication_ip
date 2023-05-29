using BulkyBookWeb.Models;

namespace GamificationAPI.Interfaces
{
    public interface IStudents
    {
        Task<List<Student>> GetStudentsAsync();
        Task<Student> GetStudentByIdAsync(int id);
        Task<Student> GetStudentByUserNameAsync(string userName);
        Task AddStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task DeleteStudentAsync(int id);
    }
}
