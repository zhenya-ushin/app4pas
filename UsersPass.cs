using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class UserData
{
    public int Id { get; set; }
    public string? ServiceName { get; set; }
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