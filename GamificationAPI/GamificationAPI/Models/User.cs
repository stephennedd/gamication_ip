﻿
using GamificationAPI.Models;
using GamificationAPI.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GamificationToIP.Models;
public class User
{
    [Key]
    public int Id { get; set; } = 0;
    public string UserId { get; set; }
    public string Password { get; set; }  
    public string VerificationCode { get; set; } = CodeGenerator.RandomString(6);
    public bool IsVerified { get; set; } = false;
    public bool IsBanned { get; set; } = false;
    public Group? Group { get; set; }
    public Role Role { get; set; }
    public ICollection<Badge> Badges { get; } = new List<Badge>();
    public List<HighScore> HighScores { get; set; } = new List<HighScore>();
}
