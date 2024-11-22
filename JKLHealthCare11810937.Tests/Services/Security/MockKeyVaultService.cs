using Azure.Security.KeyVault.Secrets;

namespace JKLHealthCare11810937.Tests.Services.Security
{
    public class MockKeyVaultService : IKeyVaultService
    {
        private readonly Dictionary<string, string> _secrets = new Dictionary<string, string>
        {
            { "MedicalRecordsEncryptionKey", "RIi9p/7hQ42fX9V+7M2R3a8oMIjhn5Q1ljAK1OT4eVc=" },
            { "DBConnectionString", "Data Source=Data/JHKHealthCare.db" },
            { "AdminUsername", "healthadmin" },
            { "AdminPassword", "Password123!" }
        };

        public string GetSecret(string secretName)
        {
            if (_secrets.TryGetValue(secretName, out string? secretValue))
            {
                return secretValue;
            }
            else
            {
                throw new InvalidOperationException($"Secret '{secretName}' not found in mock Key Vault.");
            }
        }
    }
}