using GamificationAPI.Models;

namespace GamificationAPI.Interfaces
{
    public interface IEmails
    {
        void SendEmail(EmailDto request );
    }
}
