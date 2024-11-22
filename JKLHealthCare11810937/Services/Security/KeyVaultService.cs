using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace JKLHealthCare11810937.Services.Security
{
    public class KeyVaultService : IKeyVaultService
    {
        private readonly SecretClient _secretClient;

        public KeyVaultService()
        {
            string keyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri") ?? "";
            _secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        }

        public string GetSecret(string secretName)
        {
            return _secretClient.GetSecret(secretName).Value.Value;
        }
    }

}