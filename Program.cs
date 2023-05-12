using System.Security.Cryptography;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DBRequest;
using Microsoft.IdentityModel.Tokens;

namespace DBRequest
{
    public class DatabaseContext : DbContext
    {
        public DbSet<UserData> UserDates { get; set; } = null!;
        public DatabaseContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource = DataPass.db");
        }


    }
}
namespace DBRequest
{
    public class DatabaseReg : DbContext
    {
        public DbSet<Registration> Registrations { get; set; } = null!;
        public DatabaseReg() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource = RegPass.db");
            optionsBuilder.Entity<Registration>().Totable("RegUser");
        }


    }
}
public class Registration
{
    public int Id { get; set; }
    public string Regmail { get; set; }
    public string RegPass { get; set; }
}

class RegController
{
    //написать код для добавления данных из класса Registration в отдельный файл БД
}
public class Autorithation
{
    public int Id { get; set; }
    public string LogMail { get; set; }
    public string MasterPassword { get; set; }
}

public class AutorithationController
{
    //Написать код сверяющий логин и пароль из класса Autorithation с логином и паролем из отдельного файла
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