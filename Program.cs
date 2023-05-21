using System.Security.Cryptography;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DBRequest;
using PasswordEncryption;
using System.Runtime.CompilerServices;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Net;
using MySql.Data.MySqlClient;

public partial class Program
{
    MySqlConnection conn;
    MySqlCommand cmd;
    string sql;
    public int GetCount(string nameTable)
    {
        sql = "SELECT COUNT(*) FROM" + nameTable;
        cmd = new MySqlCommand(sql, conn);
        return Convert.ToInt32(cmd.ExecuteScalar().ToString());
    }
    public void AddData(string ServiceName, string Login, string Password)
    {
        sql = "INSERT INTO DataPass (Id, ServiceName, Login, Password) ";
        sql += "Values(NULL, " + "'" +ServiceName + "', " + "'" + Login + "', " + "," + Password +  ")";
        cmd = new MySqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }
    static void Main()
    {
        MySqlCommand cmd;
        string sql;
        MySqlConnection conn;
        MySqlConnectionStringBuilder dbase;
        dbase = new MySqlConnectionStringBuilder();
        dbase.Server = "sql7.freemysqlhosting.net";
        dbase.Database = "sql7619791";
        dbase.UserID = "sql7619791";
        dbase.Password = "dNv3XIY3Ez";
        dbase.CharacterSet = "utf8";
        conn = new MySqlConnection(dbase.ConnectionString);
        try
        {
            conn.Open();
            Console.WriteLine("Подключение к БД установлено");

        }
        catch (Exception ex)
        {
            Console.WriteLine("Проблемы с подключением к БД \n\r" + ex.ToString());
        }
        sql = "Select ServiceName FROM DataPass";
        cmd = new MySqlCommand(sql, conn);
        MySqlDataReader reader = cmd.ExecuteReader();
        while(reader.Read())
        {
            //int biba = GetCount("ServiceName");
            Console.WriteLine(reader["ServiceName"].ToString() + Environment.NewLine);
        }

        reader.Close();
        // Шифрование и дешифровка введённого пользователем пароля и логина
        AESEncryptionManager manager = new();
        string servicename = "steam";
            string inplogin = "logintest"; 
            string inppass = "P45212772";
        Genpassword encK = new();
            string EncKey = encK.GenPass(16, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~`!@#$%^&*()_-+={}[].,");
            string EncIv = encK.GenPass(16, "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~`!@#$%^&*()_-+={}[].,");
            //string EncKey = "mysecretkey12345";
            //string EncIv = "1234567890123456";
            string encpass = manager.Encrypt(inppass, EncKey, EncIv);
            Console.WriteLine("InputPassword: {0}", inppass);
            Console.WriteLine("EncPassword: {0}", encpass);
            Console.WriteLine("DecPassword: {0}", manager.Decrypt(encpass, EncKey, EncIv));
            string enclogin = manager.Encrypt(inplogin, EncKey, EncIv);
            Console.WriteLine("InputLogin: {0}", inplogin);
            Console.WriteLine("EncLogin: {0}", enclogin);
            Console.WriteLine("DecLogin: {0}", manager.Decrypt(enclogin, EncKey, EncIv));
        using (DatabaseContext db = new())
        {
            using (RegKey rk = new())
            {
                // создаем два объекта
                UserData UD1 = new() { ServiceName = "Steam", Login = enclogin, Password = encpass };
                EcnryptedKey Ud1 = new() { ServiceName = "Steam", EncKey = EncKey, EncIV = EncIv };
                UserData UD2 = new() { ServiceName = "Vk", Login = "biba2", Password = "biba2" };
                rk.EncKeys.Add(Ud1);
                rk.SaveChanges();
                // добавляем объекты в бд
                db.UserDates.AddRange(UD1,UD2);
                db.SaveChanges();
                Console.WriteLine("\nObjects saved on local pc");
                //dbase.Add(UD1);
            }
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

                       
                       Console.WriteLine("\nEcnrypted iv register mail is ", EncIv);
                       
                       Console.WriteLine("\nEcnrypted key register mail is ", EncKey);
                        string ShifRegMail = manager.Encrypt(RegEmail, EncKey, EncIv);
                        Console.WriteLine("\nEcnrypted register mail is {0}", ShifRegMail);

                        Console.WriteLine("\nEcnrypted iv register mail is ", EncIv);
                      Console.WriteLine("\nEcnrypted key register mail is ", EncKey);
                        string ShifRegPassword = manager.Encrypt(RegPassword, EncKey, EncIv);
                        Console.WriteLine("\nEncrypted register password is {0}", ShifRegPassword);

                        EcnryptedKey EKey = new() { ServiceName = "Registration", EncKey = EncKey, EncIV = EncIv };
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
                        string DecRegEmail, DecRegPassword, RegEmail, RegPassword, KeyLogin,KeyPassword, IVLogin, IVPassword;
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
                                KeyLogin = k.EncKey;
                                IVLogin = k.EncIV;
                                KeyPassword = k.EncKey;
                                IVPassword = k.EncIV;

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
                                    break;
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
            default: break;
                
        }
        Console.WriteLine("What do you want to do: ");
        Console.WriteLine("1 - Save login and password ");
        Console.WriteLine("2 - Generate password ");
        Console.WriteLine("3 - Update login and password ");
        Console.WriteLine("4 - View login and password ");
        string todo = Console.ReadLine();
        switch (todo)
        {
            case "1": break;
            case "2":
                {
                    Console.WriteLine("Input length of password ");
                    int lenpass = int.Parse(Console.ReadLine());
                    Genpassword genpass = new();
                    int chtype = genpass.GenParameters(lenpass);
                    string typesymb = genpass.TypeSymbPass(chtype);
                    string Generatepassword = genpass.GenPass(lenpass, typesymb);
                    Console.WriteLine("Your Generated password is {0}", Generatepassword);
                    break;
                }
            case "3":
                {
                    using (DatabaseContext db = new())
                    {
                        using (DatabaseContext rk = new())
                        {
                            break;// string ServiceNameDB = db.UserDates.GetByServiceName()
                        }

                    }
                        break;
                }
            default: break;
        }   


    }
    }
