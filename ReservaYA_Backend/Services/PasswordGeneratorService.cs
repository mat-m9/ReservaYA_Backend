using System.Security.Cryptography;
using System.Text;

namespace ReservaYA_Backend.Services
{
    public class PasswordGeneratorService
    {
        private static readonly RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        private static readonly string capitalChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string smallChars = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string numbers = "0123456789";
        private static readonly string specialChars = "+-*?¡¿!#$%&^[]{}";
        private static readonly string AllChars = capitalChars + smallChars + numbers + specialChars;
        private static readonly PasswordGeneratorService _passwordGeneratorService = new PasswordGeneratorService();

        public static PasswordGeneratorService GetInstace() => _passwordGeneratorService;

        public async Task<string> GeneratePassword()
        {
            int length = RandomNumberGenerator.GetInt32(10, 16);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb = sb.Append(await RandomizeChars());
            }
            return sb.ToString();
        }

        private async Task<char> RandomizeChars()
        {
            byte[] byteArray = new byte[1];
            char c;
            do
            {
                RandomNumberGenerator.Create().GetBytes(byteArray);
                c = (char)byteArray[0];

            } while (!AllChars.Any(x => x == c));

            return c;
        }
    }
}
