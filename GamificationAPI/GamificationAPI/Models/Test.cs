﻿using GamificationAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace GamificationAPI.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }= null!;
        public string ImageUrl { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TimeSeconds { get; set; }
        public List<Question> Questions { get; set; } = null!;
        public virtual Subject Subject { get; set; }
    }
}
