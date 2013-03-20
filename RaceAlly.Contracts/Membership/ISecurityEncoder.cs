namespace RaceAlly.Contracts.Membership
{
    public interface ISecurityEncoder
    {

        string GenerateSalt();
        string Encode(string plainText, string salt);
        string SerializeOAuthProviderUserId(string providerName, string providerUserId);
        bool TryDeserializeOAuthProviderUserId(string protectedData, out string providerName, out string providerUserId);
    }
}
