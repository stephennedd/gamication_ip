using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class GroupReturnModel
    {
        [Key]
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public int StudentsCount { get; set; } = 0;
    }
}
