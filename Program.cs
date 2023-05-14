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
    public class RegContext : DbContext
    {
        public DbSet<Registration> Registrations { get; set; } = null!;
        public RegContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource = RegPass.db");
        }


    }
}

public class Autorithation
{
    public int Id { get; set; }
    public string LogMail { get; set; }
    public string MasterPassword { get; set; }
}

public class AutorithationController
{
    // Сверяем логин и пароль из класса Autorithation с логином и паролем из файла создающегося в RegContext
    public string ControlMail()
    {
        Console.WriteLine("Input your login: ");
        string LogMail = Console.ReadLine();
        return LogMail;
    }
    public string ControlPass()
    {
        Console.WriteLine("Input your master-password: ");
        string MasterPassword = Console.ReadLine();
        return MasterPassword;
    }
    
}
public class UserData
{
    public int Id { get; set; }
    public string? ServiceName { get; set; }
    public string Log { get; set; }
    public string Pass { get; set; }

}
public class Registration
{
    public int Id { get; set; }
    public string Regmail { get; set; }
    public string RegPass { get; set; }

    public void RegController(string args)
    {

    }
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

class  Program
{
    static void Main()
    {
        // Шифрование и дешифровка введённого пользователем пароля
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
        }
        using (RegContext rg = new RegContext())
        {
            // Записываем введённый пользователем логин и пароль во врЕменные переменные 
            Console.WriteLine("Input ur email: ");
            string RegEmail = Console.ReadLine();
            Console.WriteLine("Input ur password: ");
            string RegPassword = Console.ReadLine();

            // Шифруем введённые данные и выводим ключ на экран
            string ShifRegMail = manager.Encrypt(RegEmail);
            string ShifRegPassword = manager.Encrypt(RegPassword);
            Console.WriteLine("Ecnrypted register mail is {0}", ShifRegMail);
            Console.WriteLine("Encrypted register password is {0}", ShifRegPassword);

            // Создаём объект с зашифрованными пользовательскими данными 
            Registration REG = new Registration { Regmail = ShifRegMail, RegPass = ShifRegPassword };

            // Добавляем созданный объект в файл БД
            rg.Registrations.Add(REG);
            rg.SaveChanges();
            Console.WriteLine("Account saved ");
        }
        using (RegContext rg = new RegContext())
        {
            string DecRegEmail, DecRegPassword, RegEmail, RegPassword;
            Console.WriteLine("\nGoing to autorithation \n");

            // Получаем !построчно! данные из БД
            var regs = rg.Registrations.ToList();
            foreach (Registration r in regs)
            {
                RegEmail = r.Regmail;
                RegPassword = r.RegPass;

                // Дешифруем данные пользователя
                DecRegEmail = manager.Decrypt(RegEmail);
                DecRegPassword = manager.Decrypt(RegPassword);

                // Считываем введённые логин и пароль и сравниваем с БД
                AutorithationController AC = new AutorithationController();
                string Mail = AC.ControlMail();
                string Pass = AC.ControlPass();
                if (Mail == DecRegEmail && Pass == DecRegPassword)
                {
                    Console.WriteLine("Acces is allowed! ");
                }
                else { Console.WriteLine("Acces denied :( "); }
            }
        }
    }
}