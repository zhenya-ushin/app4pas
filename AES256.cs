using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public partial class AESEncryptionManager
{
    private const int AES_KEY_SIZE = 256; // размер ключа AES в битах
    private byte[] aesKey; // 256-битный ключ AES

    public AESEncryptionManager()
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = AES_KEY_SIZE;
            aes.GenerateKey();
            aesKey = aes.Key;
        }
    }

    // зашифровать строку
    public string Encrypt(string inptxt)
    {
        byte[] inpBytes = Encoding.UTF8.GetBytes(inptxt);
        byte[] encBytes;

        using (Aes aes = Aes.Create())
        {
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC; // режим шифрования: CBC
            aes.GenerateIV(); // случайный вектор инициализации IV
            byte[] iv = aes.IV; // сохранить IV для восстановления

            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                encBytes = encryptor.TransformFinalBlock(inpBytes, 0, inpBytes.Length);
            }
            // объединить зашифрованный текст и вектор инициализации в один массив
            byte[] encIvBytes = new byte[iv.Length + encBytes.Length];
            Array.Copy(iv, encIvBytes, iv.Length);
            Array.Copy(encBytes, 0, encIvBytes, iv.Length, encBytes.Length);

            return Convert.ToBase64String(encIvBytes);
        }
    }

    // дешифровать строку
    public string Decrypt(string enctext)
    {
        byte[] encIvBytes = Convert.FromBase64String(enctext);
        byte[] iv = new byte[16]; // IV имеет длину 16 байт
        byte[] encBytes = new byte[encIvBytes.Length - iv.Length];
        Array.Copy(encIvBytes, iv, iv.Length);
        Array.Copy(encIvBytes, iv.Length, encBytes, 0, encBytes.Length);
        byte[] inpBytes = new byte[encBytes.Length];

        using (Aes aes = Aes.Create())
        {
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC;
            aes.IV = iv;

            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                inpBytes = decryptor.TransformFinalBlock(encBytes, 0, encBytes.Length);
            }

            return Encoding.UTF8.GetString(inpBytes);
        }
    }
}
