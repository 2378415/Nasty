using System.Security.Cryptography;
using System.Text;

namespace Nasty.Common.Security
{
    public class AESHelper
    {
        public static string Encrypt(string plainText, string key)
        {
            // 检查参数
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            // 确保密钥长度正确（AES-128:16, AES-192:24, AES-256:32）
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
                throw new ArgumentException("Key must be 16, 24 or 32 bytes long");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB; // ECB模式
                aesAlg.Padding = PaddingMode.PKCS7; // PKCS7填充

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

        public static string Decrypt(string cipherText, string key)
        {
            // 检查参数
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            // 确保密钥长度正确
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length != 16 && keyBytes.Length != 24 && keyBytes.Length != 32)
                throw new ArgumentException("Key must be 16, 24 or 32 bytes long");

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.ECB; // ECB模式
                aesAlg.Padding = PaddingMode.PKCS7; // PKCS7填充

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
