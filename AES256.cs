using System.Security.Cryptography;
using System;
using System.IO;
using System.Text;
using System.Reflection.Metadata.Ecma335;
using System.CodeDom.Compiler;

namespace PasswordEncryption
{
    public partial class AESEncryptionManager
    {
       
        public  string Encrypt(string str, string key, string iv)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            using Aes aes = Aes.Create();
            {
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.BlockSize = 128;
                aes.IV = ivBytes;
                aes.Key = keyBytes;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(str);
                        }
                    }
                    byte[] encrypted = ms.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }

        }
        public string Decrypt(string cipherText, string key, string iv)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
    
//    public partial class AESEncryptionManager
//    {
//        private const int AES_KEY_SIZE = 256; // размер ключа AES в битах
//        public byte[] aesKey; // 256-битный ключ AES
//        public byte[] iv;
//        public AESEncryptionManager()
//        {

//            using Aes aes = Aes.Create();
//            aes.KeySize = AES_KEY_SIZE;
//            aes.Padding = PaddingMode.PKCS7;
//            aes.GenerateKey();
//            aesKey = aes.Key;
//            aes.GenerateIV();
//            iv = aes.IV;

//        }
//        public byte[] AESgenerateKey()
//        {

//            // return aesKey;
//            byte[] gkey = aesKey;
//            return gkey;

//        }
//        public byte[] AESgenerateIV()
//        {

//            //return
//            byte[] giv = iv;
//            return giv;
//        }

//        // зашифровать строку
//        public string Encrypt(string plaintext, byte[] gkey, byte[] giv)
//        {
//            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
//            byte[] cipherBytes;

//            using Aes aes = Aes.Create();
//            {

//                //aesKey = aes.Key;
//                aes.Key = gkey;
//                aes.Mode = CipherMode.CBC; // режим шифрования: CBC
//                aes.IV = giv;
//                byte[] iv = aes.IV; // сохранить IV для восстановления

//                using (ICryptoTransform encryptor = aes.CreateEncryptor())
//                {
//                    cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
//                }
//                // объединить зашифрованный текст и вектор инициализации в один массив
//                byte[] cipherIvBytes = new byte[iv.Length + cipherBytes.Length];
//                Array.Copy(iv, cipherIvBytes, iv.Length);
//                Array.Copy(cipherBytes, 0, cipherIvBytes, iv.Length, cipherBytes.Length);

//                return Convert.ToBase64String(cipherIvBytes);
//            }
//        }
//        // дешифровать строку
//        public string Decrypt(string ciphertext, byte[] gkey, byte[] giv)
//        {
//            using Aes aes = Aes.Create();
//            {
//                aes.Key = gkey;
//                aes.IV = giv;
//                byte[] cipherIvBytes = Convert.FromBase64String(ciphertext);
//                byte[] iv = new byte[16]; // IV имеет длину 16 байт
//                byte[] cipherBytes = new byte[cipherIvBytes.Length - iv.Length];
//                Array.Copy(cipherIvBytes, iv, iv.Length);
//                Array.Copy(cipherIvBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);
//                byte[] plainBytes = new byte[cipherBytes.Length];


//                aes.Key = aesKey;
//                aes.Mode = CipherMode.CBC;
//                aes.IV = iv;

//                using (ICryptoTransform decryptor = aes.CreateDecryptor())
//                {
//                    plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
//                }

//                return Encoding.UTF8.GetString(plainBytes);
//            }
//        }
//    }
//}
