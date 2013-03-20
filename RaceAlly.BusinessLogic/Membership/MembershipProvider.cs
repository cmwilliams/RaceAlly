using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using DotNetOpenAuth.AspNet;
using RaceAlly.Contracts.DataModel;
using RaceAlly.Contracts.Membership;
using RaceAlly.Models;
using Microsoft.Web.WebPages.OAuth;
using OAuthAccount = Microsoft.Web.WebPages.OAuth.OAuthAccount;

namespace RaceAlly.BusinessLogic.Membership
{
    public class MembershipProvider : IMembershipProvider, IOAuthProvider, IOpenAuthDataProvider
    {
        private readonly IDataContext _context;
        private readonly ISecurityEncoder _encoder;
        private readonly IApplicationEnvironment _applicationEnvironment;

        private const int TokenSizeInBytes = 16;

        private static readonly Dictionary<string, AuthenticationClientData> AuthenticationClients =
            new Dictionary<string, AuthenticationClientData>(StringComparer.OrdinalIgnoreCase);

        public MembershipProvider(IDataContext context, ISecurityEncoder encoder, IApplicationEnvironment applicationEnvironment)
        {
            _context = context;
            _encoder = encoder;
            _applicationEnvironment = applicationEnvironment;
        }

        #region Private Methods

        private User GetUserByUsername(string username)
        {
            return _context.Users.SingleOrDefault(u => u.Username == username);
        }

        private User GetUserByPasswordResetToken(string passwordResetToken)
        {
            var user = _context.Users.SingleOrDefault(u => u.PasswordResetToken == passwordResetToken);
            return user;
        }

        private User GetUserByOAuthProvider(string provider, string providerUserId)
        {
            var user = _context.Users.SingleOrDefault(u => u.OAuthAccounts.Any(a => a.Provider == provider && a.ProviderUserId == providerUserId));
            return user;
        }

        private IEnumerable<OAuthAccount> GetOAuthAccountsForUser(string username)
        {
            var user = _context.Users.Single(u => u.Username == username);
            return user.OAuthAccounts.Select(account => new OAuthAccount(account.Provider, account.ProviderUserId));
        }

        public bool DeleteOAuthAccount(string provider, string providerUserId)
        {
            var account = _context.OAuthAccounts.Find(provider, providerUserId);
            if (account != null)
            {
                _context.OAuthAccounts.Remove(account);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        #endregion

        #region Membership Members
        public bool Login(string username, string password, bool rememberMe = false)
        {
            var user = GetUserByUsername(username);
            if (user == null)
            {
                return false;
            }

            string encodedPassword = _encoder.Encode(password, user.Salt);
            bool passed = encodedPassword.Equals(user.Password);
            if (passed)
            {
                _applicationEnvironment.IssueAuthTicket(username, rememberMe);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _applicationEnvironment.RevokeAuthTicket();
        }

        public void CreateAccount(User user)
        {
            var existingUser = GetUserByUsername(user.Username);
            if (existingUser != null)
            {
                throw new MembershipException(MembershipStatus.DuplicateUserName);
            }

            user.Salt = user.Salt ?? _encoder.GenerateSalt();
            user.Password = _encoder.Encode(user.Password, user.Salt);
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateAccount(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
           
        }

        public bool HasLocalAccount(string username)
        {
            var user = GetUserByUsername(username);
            return user != null && !String.IsNullOrEmpty(user.Password);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var user = GetUserByUsername(username);
            string encodedPassword = _encoder.Encode(oldPassword, user.Salt);
            if (!encodedPassword.Equals(user.Password))
            {
                return false;
            }

            user.Password = _encoder.Encode(newPassword, user.Salt);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            return true;
        }

        public void SetLocalPassword(string username, string newPassword)
        {
            var user = GetUserByUsername(username);
            if (!String.IsNullOrEmpty(user.Password))
            {
                throw new MembershipException("SetLocalPassword can only be used on accounts that currently don't have a local password.");
            }

            user.Salt = _encoder.GenerateSalt();
            user.Password = _encoder.Encode(newPassword, user.Salt);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public string GeneratePasswordResetToken(string username, int tokenExpirationInMinutesFromNow = 1440)
        {
            var user = GetUserByUsername(username);
            if (user == null)
            {
                throw new MembershipException(MembershipStatus.InvalidUserName);
            }

            user.PasswordResetToken = GenerateToken();
            user.PasswordResetTokenExpiration = DateTime.Now.AddMinutes(tokenExpirationInMinutesFromNow);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            return user.PasswordResetToken;
        }

        public bool ResetPassword(string passwordResetToken, string newPassword)
        {
            var user = GetUserByPasswordResetToken(passwordResetToken);
            if (user == null)
            {
                return false;
            }

            if (String.IsNullOrEmpty(user.Salt))
            {
                user.Salt = _encoder.GenerateSalt();
            }
            user.Password = _encoder.Encode(newPassword, user.Salt);
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            return true;
        }

        private static string GenerateToken()
        {
            using (var prng = new RNGCryptoServiceProvider())
            {
                return GenerateToken(prng);
            }
        }

        internal static string GenerateToken(RandomNumberGenerator generator)
        {
            var tokenBytes = new byte[TokenSizeInBytes];
            generator.GetBytes(tokenBytes);
            return HttpServerUtility.UrlTokenEncode(tokenBytes);
        }

        public static void RegisterClient(IAuthenticationClient client,
                                          string displayName, IDictionary<string, object> extraData)
        {
            var clientData = new AuthenticationClientData(client, displayName, extraData);
            AuthenticationClients.Add(client.ProviderName, clientData);
        }

        #endregion

        #region OAuthProvider Members
        
        public ICollection<AuthenticationClientData> RegisteredClientData
        {
            get { return AuthenticationClients.Values; }
        }

        public bool OAuthLogin(string provider, string providerUserId, bool persistCookie)
        {
            AuthenticationClientData oauthProvider = AuthenticationClients[provider];
            HttpContextBase context = _applicationEnvironment.AcquireContext();
            var securityManager = new OpenAuthSecurityManager(context, oauthProvider.AuthenticationClient, this);
            return securityManager.Login(providerUserId, persistCookie);
        }

        public void RequestOAuthAuthentication(string provider, string returnUrl)
        {
            AuthenticationClientData client = AuthenticationClients[provider];
            _applicationEnvironment.RequestAuthentication(client.AuthenticationClient, this, returnUrl);
        }

        public AuthenticationResult VerifyOAuthAuthentication(string returnUrl)
        {
            string providerName = _applicationEnvironment.GetOAuthPoviderName();
            if (String.IsNullOrEmpty(providerName))
            {
                return AuthenticationResult.Failed;
            }

            AuthenticationClientData client = AuthenticationClients[providerName];
            return _applicationEnvironment.VerifyAuthentication(client.AuthenticationClient, this, returnUrl);
        }

        public void CreateOAuthAccount(string provider, string providerUserId, User user)
        {
            var existingUser = GetUserByUsername(user.Username);
            if (existingUser == null)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            user = existingUser ?? _context.Users.Single(u => u.Username == user.Username);
           
            if(user.OAuthAccounts == null)
            {
                user.OAuthAccounts = new EntityCollection<Models.OAuthAccount>();
            }
            user.OAuthAccounts.Add(new Models.OAuthAccount() { Provider = provider, ProviderUserId = providerUserId});
            _context.SaveChanges();

        }

        public bool DisassociateOAuthAccount(string provider, string providerUserId)
        {
            var user = GetUserByOAuthProvider(provider, providerUserId);
            if (user == null)
            {
                return false;
            }
            IEnumerable<OAuthAccount> accounts = GetOAuthAccountsForUser(user.Username);

            if (HasLocalAccount(user.Username))
                return DeleteOAuthAccount(provider, providerUserId);

            if (accounts.Count() > 1)
                return DeleteOAuthAccount(provider, providerUserId);

            return false;
        }

        public AuthenticationClientData GetOAuthClientData(string provider)
        {
            return AuthenticationClients[provider];
        }

        public IEnumerable<OAuthAccount> GetOAuthAccountsFromUserName(string username)
        {
            return GetOAuthAccountsForUser(username);
        }
        #endregion

        #region IOpenAuthDataProvider Members

        public string GetUserNameFromOpenAuth(string provider, string providerUserId)
        {
            var user = GetUserByOAuthProvider(provider, providerUserId);
            if (user != null)
            {
                return user.Username;
            }
            return String.Empty;
        }

        #endregion
    }
}
