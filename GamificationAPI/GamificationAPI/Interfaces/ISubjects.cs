

using GamificationAPI.Models;

namespace GamificationAPI.Interfaces

{
    public interface ISubjects
    {
        Task<List<Subject>> GetSubjects();

        Task<Subject> AddSubject(NewSubject newSubject);
        Task<RootObject> UpdateTables(RootObject data);

        Task<Subject> DeleteSubject(int id);
    }
}
