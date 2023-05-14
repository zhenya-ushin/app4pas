public partial class UserData
{
    public int Id { get; set; }
    public string ServiceName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }

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
    public byte[] EncKeyLogin { get; set; }
    public byte[] EncIVLogin { get; set; }
    public byte[] EncKeyPassword { get; set; }
    public byte[] EncIVPassword { get; set; }

}