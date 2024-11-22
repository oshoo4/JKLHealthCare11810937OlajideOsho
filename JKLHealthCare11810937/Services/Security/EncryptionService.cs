using System.Security.Cryptography;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace JKLHealthCare11810937.Services.Security
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly IKeyVaultService _keyVaultService;

        public EncryptionService(IKeyVaultService keyVaultService)
        {
            _keyVaultService = keyVaultService;
            _key = GetEncryptionKey();
        }

        private byte[] GetEncryptionKey()
        {
            string keyValue = _keyVaultService.GetSecret("MedicalRecordsEncryptionKey");

            if (keyValue != null && keyValue.Length > 0)
            {
                return Convert.FromBase64String(keyValue);
            }
            else
            {
                throw new InvalidOperationException("Encryption key 'MedicalRecordsEncryptionKey' not found in appsettings.json.");
            }
        }

        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        var encrypted = msEncrypt.ToArray();
                        return Convert.ToBase64String(aesAlg.IV.Concat(encrypted).ToArray());
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] iv = new byte[16];
            byte[] cipher = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipher))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}