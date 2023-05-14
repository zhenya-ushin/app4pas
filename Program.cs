using System.Security.Cryptography;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DBRequest;

namespace DBRequest
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<UserData> UserDates { get; set; } = null!;
        public DatabaseContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource = DataPass.db");
        }
    }
    public partial class RegContext : DbContext
    {
        public DbSet<Registration> Registrations { get; set; } = null!;
        public RegContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource = RegPass.db");
        }


    }
}

class Program
    {
        static void Main()
        {
            // Шифрование и дешифровка введённого пользователем пароля
            AESEncryptionManager manager = new();
            string inptxt = "P4$$w0Rd5212772";
            string enctxt = manager.Encrypt(inptxt);
            Console.WriteLine("InputPassword: {0}", manager.Decrypt(enctxt));
            Console.WriteLine("EncPassword: {0}", enctxt);
            Console.WriteLine("DecPassword: {0}", manager.Decrypt(enctxt));
            using (DatabaseContext db = new())
            {
                // создаем два объекта
                UserData UD1 = new () { ServiceName = "Steam", Login = "biba1", Password = "biba1" };
                UserData UD2 = new () { ServiceName = "Vk", Login = "biba2", Password = "biba2" };

                // добавляем объекты в бд
                db.UserDates.Add(UD1);
                db.UserDates.Add(UD2);
                db.SaveChanges();
                Console.WriteLine("Objects saved");
            }
            using (RegContext rg = new())
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
                Registration REG = new() { Regmail = ShifRegMail, RegPass = ShifRegPassword };

                // Добавляем созданный объект в файл БД
                rg.Registrations.Add(REG);
                rg.SaveChanges();
                Console.WriteLine("Account saved ");
            }
            using (RegContext rg = new())
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
                    AutorithationController AC = new();
                    string Mail = AutorithationController.ControlMail();
                    string Pass = AutorithationController.ControlPass();
                    if (Mail == DecRegEmail && Pass == DecRegPassword)
                    {
                        Console.WriteLine("Acces is allowed! ");
                    }
                    else 
                { 
                    Console.WriteLine("Acces denied :( "); 

                }
                }
            }
        }
    }
