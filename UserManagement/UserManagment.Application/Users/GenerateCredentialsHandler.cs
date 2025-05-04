using System.Text;
namespace UserManagment.Application.Users
{
    public class UserCredentialGenerator
    {
        public (string UserName, string Password) GenerateRandomUserCredentials()
        {
            return GetRandomUserNameAndPassword();
        }

        private string GenerateRandomUsername()
        {
            var random = new Random();
            var lowerChars = new string(Enumerable.Range(0, 4).Select(_ => (char)random.Next('a', 'z' + 1)).ToArray());
            var upperChars = new string(Enumerable.Range(0, 4).Select(_ => (char)random.Next('A', 'Z' + 1)).ToArray());
            return $"{lowerChars}{upperChars}";
        }

        public string GenerateRandomPassword()
        {
            var chars = "0123456789abcdefghijklmnopqrstuvwxyz!@#$%";
            var output = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                output.Append(chars[random.Next(chars.Length)]);
            }
            return output.ToString();
        }

        private (string userName, string password) GetRandomUserNameAndPassword()
        {
            var randomUsername = GenerateRandomUsername();
            var randomPassword = GenerateRandomPassword();
            return (randomUsername, randomPassword);
        }
    }
}