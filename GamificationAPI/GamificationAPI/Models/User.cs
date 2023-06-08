
using GamificationAPI.Models;
using GamificationAPI.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamificationToIP.Models;
public class User
{
    [Key]
    public string Id { get; set; }
    public string Password { get; set; }  
    public string VerificationCode { get; set; } = CodeGenerator.RandomString(6);
    public bool IsVerified { get; set; } = false;
    public bool IsBanned { get; set; } = false;

    public Group Group { get; set; }

    public Role Role { get; set; }

    public List<HighScore> HighScores { get; set; } = new List<HighScore>();
}
