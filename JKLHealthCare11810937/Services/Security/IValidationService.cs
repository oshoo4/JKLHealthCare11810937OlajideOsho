namespace JKLHealthCare11810937.Services.Security
{
    public interface IValidationService
    {
        bool IsPasswordComplex(string password);
        public bool IsValidContact(string contact);
        bool IsValidEmail(string contact);
    }
}