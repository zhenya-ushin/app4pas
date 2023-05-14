using System.Security.Cryptography;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DBRequest;
using PasswordEncryption;
using System.Runtime.CompilerServices;

class Program
    {
        static void Main()
        {
        // Шифрование и дешифровка введённого пользователем пароля
        AESEncryptionManager manager = new();
            string inptxt = "P4$$w0Rd5212772";
            byte[] EncKey = manager.AESgenerateKey();
            byte[] EncIv = manager.AESgenerateIV();
            string enctxt = manager.Encrypt(inptxt, EncKey, EncIv);
            Console.WriteLine("InputPassword: {0}", inptxt);
            Console.WriteLine("EncPassword: {0}", enctxt);
            Console.WriteLine("DecPassword: {0}", manager.Decrypt(enctxt, EncKey, EncIv));
            using (DatabaseContext db = new())
            {
                // создаем два объекта
                UserData UD1 = new () { ServiceName = "Steam", Login = "biba1", Password = "biba1" };
                UserData UD2 = new () { ServiceName = "Vk", Login = "biba2", Password = "biba2" };

                // добавляем объекты в бд
                db.UserDates.Add(UD1);
                db.UserDates.Add(UD2);
                db.SaveChanges();
                Console.WriteLine("\nObjects saved");
            }
        Console.WriteLine("Registration account? (yes/no)\n ");
        string boolreg = Console.ReadLine();
        switch(boolreg)
        {
            case "yes":
                using (RegContext rg = new())
                {
                    // Записываем введённый пользователем логин и пароль во врЕменные переменные 
                    Console.WriteLine("Input ur email: \n");
                    string RegEmail = Console.ReadLine();
                    Console.WriteLine("\nInput ur password: \n");
                    string RegPassword = Console.ReadLine();

                    using (RegKey rk = new())
                    {
                        // Шифруем введённые данные и выводим ключ на экран

                        byte[] EncRegMailIV =manager.AESgenerateIV();
                        Console.WriteLine("\nEcnrypted iv register mail is ", EncRegMailIV);
                        byte[] EncRegMailKey = manager.AESgenerateKey();
                        Console.WriteLine("\nEcnrypted key register mail is ", EncRegMailKey);
                        string ShifRegMail = manager.Encrypt(RegEmail, EncRegMailKey, EncRegMailIV);
                        Console.WriteLine("\nEcnrypted register mail is {0}", ShifRegMail);

                        byte[] EncRegPassIV = manager.AESgenerateIV();
                        Console.WriteLine("\nEcnrypted iv register mail is ", EncRegPassIV);
                        byte[] EncRegPassKey = manager.AESgenerateKey();
                        Console.WriteLine("\nEcnrypted key register mail is ", EncRegPassKey);
                        string ShifRegPassword = manager.Encrypt(RegPassword, EncRegPassKey, EncRegPassIV);
                        Console.WriteLine("\nEncrypted register password is {0}", ShifRegPassword);

                        EcnryptedKey EKey = new() { ServiceName = "Registration", EncKeyLogin = EncRegMailKey, EncIVLogin = EncRegMailIV, EncKeyPassword = EncRegPassKey, EncIVPassword = EncRegPassIV };
                        rk.EncKeys.Add(EKey);
                        rk.SaveChanges();
                        // Создаём объект с зашифрованными пользовательскими данными 
                        Registration REG = new() { Regmail = ShifRegMail, RegPass = ShifRegPassword };

                        // Добавляем созданный объект в файл БД
                        rg.Registrations.Add(REG);
                        rg.SaveChanges();
                        Console.WriteLine("\nAccount saved ");
                    }
                }
                goto case "no";
            case "no":
                using (RegContext rg = new())
                {
                    using (RegKey rk = new())
                    {
                        string DecRegEmail, DecRegPassword, RegEmail, RegPassword; 
                        byte[] KeyLogin,KeyPassword, IVLogin, IVPassword;
                        Console.WriteLine("\nGoing to autorithation \n");

                        // Получаем !построчно! данные из БД
                        var regs = rg.Registrations.ToList();
                        foreach (Registration r in regs)
                        {
                            RegEmail = r.Regmail;
                            RegPassword = r.RegPass;
                            var rkey = rk.EncKeys.ToList();
                            foreach (EcnryptedKey k in rkey)
                            {
                                KeyLogin = k.EncKeyLogin;
                                IVLogin = k.EncIVLogin;
                                KeyPassword = k.EncKeyPassword;
                                IVPassword = k.EncIVPassword;

                                // Дешифруем данные пользователя
                                DecRegEmail = manager.Decrypt(RegEmail, KeyLogin, IVLogin);
                                DecRegPassword = manager.Decrypt(RegPassword, KeyPassword, IVPassword);

                                // Считываем введённые логин и пароль и сравниваем с БД
                                AutorithationController AC = new();
                                string Mail = AutorithationController.ControlMail();
                                string Pass = AutorithationController.ControlPass();
                                if (Mail == DecRegEmail && Pass == DecRegPassword)
                                {
                                    Console.WriteLine("\nAcces is allowed! ");
                                }
                                else
                                {
                                    Console.WriteLine("\nAcces denied :( ");

                                }
                            }
                        }
                    }
                }
                break;

        }

            
           
        }
    }
