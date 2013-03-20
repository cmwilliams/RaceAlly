using System.Collections.Generic;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using RaceAlly.Models;
using OAuthAccount = Microsoft.Web.WebPages.OAuth.OAuthAccount;

namespace RaceAlly.Contracts.Membership
{
    public interface IOAuthProvider
    {
        ICollection<AuthenticationClientData> RegisteredClientData { get; }
        bool OAuthLogin(string provider, string providerUserId, bool persistCookie);
        void RequestOAuthAuthentication(string provider, string returnUrl);
        AuthenticationResult VerifyOAuthAuthentication(string action);
        void CreateOAuthAccount(string provider, string providerUserId, User user);
        bool DisassociateOAuthAccount(string provider, string providerUserId);
        AuthenticationClientData GetOAuthClientData(string provider);
        IEnumerable<OAuthAccount> GetOAuthAccountsFromUserName(string username);

    }
}
