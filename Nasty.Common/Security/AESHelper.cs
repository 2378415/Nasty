using System.Security.Cryptography;
using System.Text;

namespace Nasty.Common.Security
{
    public class AESHelper
    {
        public static byte[] StringTo256BitHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                return sha256.ComputeHash(inputBytes); // 返回 32 字节数组
            }
        }

        /// <summary>
        /// AES/CBC/PKCS7/IVIsNull 加密 变种版本（将密钥二次256哈希）
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Encrypt(string plainText, string key)
        {
            // 检查参数
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            byte[] keyBytes = StringTo256BitHash(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.CBC; // CBC模式
                aesAlg.Padding = PaddingMode.PKCS7; // PKCS7填充
                aesAlg.IV = new byte[16]; // 全零 IV

                // 创建加密器
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // 加密数据
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                        csEncrypt.FlushFinalBlock();
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// AES/CBC/PKCS7/IVIsNull 解密 变种版本（将密钥二次256哈希）
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string Decrypt(string cipherText, string key)
        {
            // 检查参数
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            byte[] keyBytes = StringTo256BitHash(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.CBC; // CBC模式
                aesAlg.Padding = PaddingMode.PKCS7; // PKCS7填充
                aesAlg.IV = new byte[16]; // 全零 IV

                // 创建解密器
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // 解密数据
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
