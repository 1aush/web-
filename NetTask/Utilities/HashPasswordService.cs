using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace NetTask.Utilities
{
    public class HashPasswordService
    {
        //创建随机Salt
        public static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        //加密
        public static string HashPassword(string password, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 20,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }

        //验证
        public static bool Validate(string password, string salt, string passwordStore)
        {
            if (string.IsNullOrEmpty(password)) return false;
            if (string.IsNullOrEmpty(salt)) return false;
            if (string.IsNullOrEmpty(passwordStore)) return false;

            return HashPassword(password, salt) == passwordStore;
        }
    }
}
