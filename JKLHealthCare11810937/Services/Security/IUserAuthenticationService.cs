namespace JKLHealthCare11810937.Services.Security
{
    public interface IUserAuthenticationService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}