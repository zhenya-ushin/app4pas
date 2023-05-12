using System.Security.Cryptography;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DBRequest;

namespace DBRequest
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserData> UserDates { get; set; } = null!;
        public DatabaseContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource = helloapp.db");
        }


    }
}

public class UserData
{
    public int Id { get; set; }
    public string? ServiceName { get; set; }
    public string Log { get; set; }
    public string Pass { get; set; }

}
public class PasswordController
{
    private string fileName;

    public PasswordController(string fileName)
    {
        this.fileName = fileName;
    }
    public void AddPassword(string login, string password)
    {
        using (StreamWriter sw = File.AppendText(fileName))
        {
            sw.WriteLine($"{login}:{password}");
        }
    }
    public List<string> GetPassword()
    {
        List<string> passwords = new List<string>();
        using (StreamReader sr = new StreamReader(fileName))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                passwords.Add(line);
            }
        }
        return passwords;
    }
}

public class AESEncryptionManager
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
    public string Encrypt(string plaintext)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] cipherBytes;

        using (Aes aes = Aes.Create())
        {
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC; // режим шифрования: CBC
            aes.GenerateIV(); // случайный вектор инициализации IV
            byte[] iv = aes.IV; // сохранить IV для восстановления

            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }
            // объединить зашифрованный текст и вектор инициализации в один массив
            byte[] cipherIvBytes = new byte[iv.Length + cipherBytes.Length];
            Array.Copy(iv, cipherIvBytes, iv.Length);
            Array.Copy(cipherBytes, 0, cipherIvBytes, iv.Length, cipherBytes.Length);

            return Convert.ToBase64String(cipherIvBytes);
        }
    }

    // дешифровать строку
    public string Decrypt(string ciphertext)
    {
        byte[] cipherIvBytes = Convert.FromBase64String(ciphertext);
        byte[] iv = new byte[16]; // IV имеет длину 16 байт
        byte[] cipherBytes = new byte[cipherIvBytes.Length - iv.Length];
        Array.Copy(cipherIvBytes, iv, iv.Length);
        Array.Copy(cipherIvBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);
        byte[] plainBytes = new byte[cipherBytes.Length];

        using (Aes aes = Aes.Create())
        {
            aes.Key = aesKey;
            aes.Mode = CipherMode.CBC;
            aes.IV = iv;

            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}

// пример использования
class Program
{
    public async Task<List<UserData>> GetData()
    {
        using (var db = new DatabaseContext())
        {
            var data = await db.UserDates.ToListAsync();
            return data;
        }
    }

    static void Main()
    {
        AESEncryptionManager manager = new AESEncryptionManager();
        string inptxt = "P4$$w0Rd5212772";
        string enctxt = manager.Encrypt(inptxt);
        string dectxt = manager.Decrypt(enctxt);
        Console.WriteLine("InputPassword: {0}", manager.Decrypt(enctxt));
        Console.WriteLine("EncPassword: {0}", enctxt);
        Console.WriteLine("DecPassword: {0}", manager.Decrypt(enctxt));
        using (DatabaseContext db = new DatabaseContext()) 
        {
            // создаем два объекта
            UserData UD1 = new UserData { ServiceName = "Steam", Log = "biba1", Pass = "biba1" };
            UserData UD2 = new UserData { ServiceName = "Vk", Log = "biba2", Pass = "biba2" };

            // добавляем объекты в бд
            db.UserDates.Add(UD1);
            db.UserDates.Add(UD2);
            db.SaveChanges();
            Console.WriteLine("Objects saved");
            //GetData

            // получаем объекты из бд и выводим в консоль
           // var userDates = db.UserDates.ToList();
           // Console.WriteLine("list of object");
           // foreach (UserDate u in userDates)
           // {
           //     Console.WriteLine($"{u.Id}.{u.ServiceName} - {u.Log} - {u.Pass}");
           // }

        }
    }
}