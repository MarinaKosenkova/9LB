using System.Text.RegularExpressions;

namespace Desktop
{
    public class InputValidator
    {
        public bool IsValidEmail(string mail)
        {
            return Regex.IsMatch(mail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public bool IsValidPassword(string password)
        {
            return password.Length >= 6;
        }

        public bool IsValidName(string name)
        {
            return name.Length >= 3;
        }
    }
}
