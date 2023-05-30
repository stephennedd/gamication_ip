using BulkyBookWeb.Models;
using GamificationAPI.Interfaces;
using GamificationToIP.Context;
using Microsoft.EntityFrameworkCore;

public class StudentService : IStudents
{
    private readonly ApplicationDbContext _dbContext;

    public StudentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Student> GetStudentByIdAsync(int id)
    {
        return await _dbContext.Set<Student>()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> GetStudentByEmailAsync(string email)
    {
        return await _dbContext.Set<Student>()
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task AddStudentAsync(Student student)
    {
        _dbContext.Set<Student>().Add(student);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateStudentAsync(Student student)
    {
        _dbContext.Set<Student>().Update(student);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteStudentAsync(int id)
    {
        var student = await GetStudentByIdAsync(id);

        if (student != null)
        {
            _dbContext.Set<Student>().Remove(student);
            await _dbContext.SaveChangesAsync();
        }
    }

    public Task<List<Student>> GetStudentsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Student> GetStudentByUserNameAsync(string userName)
    {
        throw new NotImplementedException();
    }
}