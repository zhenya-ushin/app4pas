using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class AutorithationController
{
    // Сверяем логин и пароль из класса Autorithation с логином и паролем из файла создающегося в RegContext
    public static string ControlMail()
    {
        Console.WriteLine("Input your login: ");
        string LogMail = Console.ReadLine();
        return LogMail;
    }
    public static string ControlPass()
    {
        Console.WriteLine("Input your master-password: ");
        string MasterPassword = Console.ReadLine();
        return MasterPassword;
    }

}
