public partial class UserData
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string? CardNumber { get; set; }
    public string? CardCVV { get; set; }
    public string? Description { get; set; }
    public string GetServiceName()
    {
        Console.WriteLine("Input service name");
        string servicename = Console.ReadLine();
        return servicename;
    }
    public string GetLogin()
    {
        Console.WriteLine("Input login");
        string login = Console.ReadLine();
        return login;
    }
    public string GetPassword()
    {
        Console.WriteLine("Input password");
        string password = Console.ReadLine();
        return password;
    }
}

public partial class Registration
{
    public int Id { get; set; }
    public string Regmail { get; set; }
    public string RegPass { get; set; }

}
public partial class Autorization
{
    public int Id { get; set; }
    public string LogMail { get; set; }
    public string MasterPassword { get; set; }
}

public partial class EcnryptedKey
{
    public int Id { get; set; } 
    public string ServiceName { get; set; }
    public string EncKey { get; set; }
    public string EncIV { get; set; }


}
namespace datapa
{
    public partial class Data
    {
        public string GetServiceName()
        {
            Console.WriteLine("Input service name");
            string servicename = Console.ReadLine();
            return servicename;
        }
        public string GetLogin()
        {
            Console.WriteLine("Input login");
            string login = Console.ReadLine();
            return login;
        }
        public string GetPassword()
        {
            Console.WriteLine("Input password");
            string password = Console.ReadLine();
            return password;
        }
    }
}