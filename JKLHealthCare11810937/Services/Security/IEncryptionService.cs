namespace JKLHealthCare11810937.Services.Security
{
    public interface IEncryptionService
    {
        public string Encrypt(string plainText);
        public string Decrypt(string cipherText);
    }
}