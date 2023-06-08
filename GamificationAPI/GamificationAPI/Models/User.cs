
using GamificationAPI.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamificationToIP.Models;
public class User
{
    [Key]
    public string Id { get; set; }
    public string Password { get; set; }
    public string? Group { get; set; } = string.Empty;
    public bool IsBanned { get; set; } = false;
    public bool IsVerified { get; set; } = false;

    public Role Role { get; set; }

    public List<HighScore> HighScores { get; set; } = new List<HighScore>();
}
