using RaceAlly.Models;

namespace RaceAlly.Contracts.Membership
{
    public interface IMembershipProvider
    {
        bool Login(string username, string password, bool rememberMe = false);
        void Logout();
        void CreateAccount(User user);
        void UpdateAccount(User user);
        bool HasLocalAccount(string username);
        bool ChangePassword(string username, string oldPassword, string newPassword);
        void SetLocalPassword(string username, string newPassword);
        string GeneratePasswordResetToken(string username, int tokenExpirationInMinutesFromNow = 1440);
        bool ResetPassword(string passwordResetToken, string newPassword);
    }
}
