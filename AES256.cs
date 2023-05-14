using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
public partial class AESEncryptionManager
{
    private readonly byte[] key = new byte[32] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20 };
    private readonly byte[] iv = new byte[16] { 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30 };

    public string Encrypt(string input)
    {
        using (var aes = Aes.Create())
        using (var encryptor = aes.CreateEncryptor(key, iv))
        using (var ms = new MemoryStream())
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(input);
            writer.Flush();
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
    }

    public string Decrypt(string input)
    {
        using (var aes = Aes.Create())
        using (var decryptor = aes.CreateDecryptor(key, iv))
        using (var ms = new MemoryStream(Convert.FromBase64String(input)))
        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
        using (var reader = new StreamReader(cs))
        {
            return reader.ReadToEnd();
        }
    }
}
//public partial class AESEncryptionManager
//{
//    private const int AES_KEY_SIZE = 256; // размер ключа AES в битах
//    private byte[] aesKey; // 256-битный ключ AES

//    public AESEncryptionManager()
//    {
//        using Aes aes = Aes.Create();
//        aes.KeySize = AES_KEY_SIZE;
//        aes.GenerateKey();
//        aesKey = aes.Key;
//    }

//    // зашифровать строку
//    public string Encrypt(string inptxt)
//    {
//        byte[] inpBytes = Encoding.UTF8.GetBytes(inptxt);
//        byte[] encBytes;

//        using Aes aes = Aes.Create();
//        aes.Key = aesKey;
//        aes.Mode = CipherMode.CBC; // режим шифрования: CBC
//        aes.GenerateIV(); // случайный вектор инициализации IV
//        byte[] iv = aes.IV; // сохранить IV для восстановления

//        using (ICryptoTransform encryptor = aes.CreateEncryptor())
//        {
//            encBytes = encryptor.TransformFinalBlock(inpBytes, 0, inpBytes.Length);
//        }
//        // объединить зашифрованный текст и вектор инициализации в один массив
//        byte[] encIvBytes = new byte[iv.Length + encBytes.Length];
//        Array.Copy(iv, encIvBytes, iv.Length);
//        Array.Copy(encBytes, 0, encIvBytes, iv.Length, encBytes.Length);

//        return Convert.ToBase64String(encIvBytes);
//    }

//    // дешифровать строку
//    public string Decrypt(string enctext)
//    {
//        byte[] encIvBytes = Convert.FromBase64String(enctext);
//        byte[] iv = new byte[16]; // IV имеет длину 16 байт
//        byte[] encBytes = new byte[encIvBytes.Length - iv.Length];
//        Array.Copy(encIvBytes, iv, iv.Length);
//        Array.Copy(encIvBytes, iv.Length, encBytes, 0, encBytes.Length);
//        byte[] inpBytes = new byte[encBytes.Length];

//        using Aes aes = Aes.Create();
//        aes.Key = aesKey;
//        aes.Mode = CipherMode.CBC;
//        aes.IV = iv;

//        using (ICryptoTransform decryptor = aes.CreateDecryptor())
//        {
//            inpBytes = decryptor.TransformFinalBlock(encBytes, 0, encBytes.Length);
//        }

//        return Encoding.UTF8.GetString(inpBytes);
//    }
//}
