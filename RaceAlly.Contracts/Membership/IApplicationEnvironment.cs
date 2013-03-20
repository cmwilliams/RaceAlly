﻿using System.Web;
using DotNetOpenAuth.AspNet;

namespace RaceAlly.Contracts.Membership
{
    public interface IApplicationEnvironment
    {
        void IssueAuthTicket(string username, bool persist);
        void RevokeAuthTicket();
        string GetOAuthPoviderName();
        void RequestAuthentication(IAuthenticationClient client, IOpenAuthDataProvider provider, string returnUrl);
        AuthenticationResult VerifyAuthentication(IAuthenticationClient client, IOpenAuthDataProvider provider,
                                                string returnUrl);
        HttpContextBase AcquireContext();
    }
}
