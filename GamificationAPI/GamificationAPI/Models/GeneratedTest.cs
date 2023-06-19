using GamificationToIP.Models;

namespace GamificationAPI.Models

{

    public class GeneratedTest

    {

        public int Id { get; set; }

        public int StudentId { get; set; }

        public int TestId { get; set; }

        public int numberOfCorrectAnswers { get; set; } = 0;

        public User student { get; set; }

        public Test Test { get; set; }

    }

}
