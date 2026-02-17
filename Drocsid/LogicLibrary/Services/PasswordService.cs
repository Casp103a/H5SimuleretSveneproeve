using System.Text;

namespace LogicLibrary.Services
{
    public class PasswordService
    {
        public string EncodePassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }

        public string DecodePassword(string? password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }

            var bytes = Convert.FromBase64String(password);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
