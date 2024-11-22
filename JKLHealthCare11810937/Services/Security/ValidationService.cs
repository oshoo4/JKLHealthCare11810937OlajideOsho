using System.Text.RegularExpressions;

namespace JKLHealthCare11810937.Services.Security
{
    public class ValidationService : IValidationService
    {
        public bool IsPasswordComplex(string password)
        {
            bool hasLowercase = Regex.IsMatch(password, "[a-z]");
            bool hasUppercase = Regex.IsMatch(password, "[A-Z]");
            bool hasDigits = Regex.IsMatch(password, @"\d");
            bool hasSpecialChars = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]");
            bool hasMinimumLength = password.Length >= 8;

            return hasLowercase && hasUppercase && hasDigits && hasSpecialChars && hasMinimumLength;
        }

        public bool IsValidContact(string contact)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            string phonePattern = @"^\d{6,13}$";

            return Regex.IsMatch(contact, emailPattern) || Regex.IsMatch(contact, phonePattern);
        }

        public bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, emailPattern);
        }
    }
}