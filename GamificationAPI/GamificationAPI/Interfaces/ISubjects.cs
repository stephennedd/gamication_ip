

using Microsoft.AspNetCore.Mvc;

namespace GamificationAPI.Interfaces

{
    public interface ISubjects
    {
        Task<List<Subject>> GetSubjects();
        Task<RootObject> UpdateTables(RootObject data);
    }

}
