﻿using System.Web;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using RaceAlly.Contracts.Membership;

namespace RaceAlly.BusinessLogic.Membership
{
    public class AspnetEnvironment : IApplicationEnvironment
    {
        public void IssueAuthTicket(string username, bool persist)
        {
            FormsAuthentication.SetAuthCookie(username, persist);
        }

        public void RevokeAuthTicket()
        {
            FormsAuthentication.SignOut();
        }

        public HttpContextBase AcquireContext()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }

        public void RequestAuthentication(IAuthenticationClient client, IOpenAuthDataProvider provider, string returnUrl)
        {
            var securityManager = new OpenAuthSecurityManager(
                new HttpContextWrapper(HttpContext.Current), client, provider);
            securityManager.RequestAuthentication(returnUrl);
        }

        public string GetOAuthPoviderName()
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            return OpenAuthSecurityManager.GetProviderName(context);
        }

        public AuthenticationResult VerifyAuthentication(IAuthenticationClient client, IOpenAuthDataProvider provider,
                                                         string returnUrl)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            var securityManager = new OpenAuthSecurityManager(context, client, provider);
            return securityManager.VerifyAuthentication(returnUrl);
        }
    }
}
