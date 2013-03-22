using System;
using System.Collections.Generic;

namespace RaceAlly.Models
{
    public class User
    {
        public User()
        {
            PasswordResetTokenExpiration = DateTime.Now;
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Salt { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime PasswordResetTokenExpiration { get; set; }
        public bool IsLocal { get; set; }
        public virtual ICollection<OAuthAccount> OAuthAccounts { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Race> Races { get; set; }
    }
}
