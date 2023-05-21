using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
partial class Genpassword {
    public int GenParameters(int NumberOfSymbol) {
        Console.WriteLine("Choose type of symbol for generation password \n");
        Console.WriteLine("1 - UpLetters and Numbers \n");
        Console.WriteLine("2 - LowLetters and Numbers \n");
        Console.WriteLine("3 - AllLetters and Numbers and Symbols \n");
        Console.WriteLine("Input your type ");
        int ChType = int.Parse(Console.ReadLine());
        return ChType;
    }
   public string TypeSymbPass(int ChType)
    {
        switch (ChType)
        { 
            case 1:
            return "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            case 2: 
            return "0123456789abcdefghijklmnopqrstuvwxyz";
            case 3:
            return "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~`!@#$%^&*()_-+={}[].,";
            default: return "Invalid Type";

        }
    }
   public string GenPass(int NumberOfSymbol, string typesymb)
    {
        Random random = new Random();
        return new string(Enumerable.Repeat(typesymb, NumberOfSymbol).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

