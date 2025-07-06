using System.Security.Cryptography;
using System.Text;

namespace FriChat.Core.Common
{
    public class EncryptionHelper
    {
        // Use guaranteed-correct key and IV lengths for AES-256
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();   
            aes.Key = Key;
            aes.IV = IV;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            var buffer = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(buffer);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
