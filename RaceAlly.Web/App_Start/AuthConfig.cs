using System.Collections.Generic;
using DotNetOpenAuth.AspNet.Clients;
using Microsoft.Web.WebPages.OAuth;
using RaceAlly.BusinessLogic.Membership;

namespace RaceAlly.Web
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            MembershipProvider.RegisterClient(
                new FacebookClient("625721824109641", "12f28b366c7c3288be79ed2a6ce35f3d"),
                "Facebook", new Dictionary<string, object>());

            MembershipProvider.RegisterClient(
                 new GoogleOpenIdClient(),
                 "Google", new Dictionary<string, object>());
        }
    }
}
