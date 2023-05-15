using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBRequest;

namespace DBRequest
{
    // Создаём контекст для базы данных 
    public partial class DatabaseContext : DbContext
    {
        // Определяем множество объектов этой базы данных
        public DbSet<UserData> UserDates { get; set; } = null!;

        // Создаём файл если его не существует
        public DatabaseContext() => Database.EnsureCreated();

        // Определяем имя файла
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
    public partial class RegKey : DbContext
    {
        public DbSet<EcnryptedKey> EncKeys { get; set; } = null!;
        public RegKey() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource = EncKeys.db");
        }


    }
}

public partial class UserDataRep
{
    private readonly DatabaseContext databaseContext;
    public UserDataRep()
    {
        databaseContext = new DatabaseContext();
    }
    public UserData GetByServiceName(string ServiceName)
    {
        return databaseContext.UserDates.FirstOrDefault(u => u.ServiceName == ServiceName);
    }
    public void CreateUser(UserData userData)
    {
        databaseContext.UserDates.Add(userData);
        databaseContext.SaveChanges();
    }
    public void UpdateUser(UserData userData)
    {
        var searchUser = GetByServiceName(userData.ServiceName);
        if (searchUser != null)
        {
            searchUser.Login = userData.Login;
            searchUser.Password = userData.Password;
            databaseContext.SaveChanges();
        }
    }
    public void DeleteUser(string ServiceName)
    {
        var SearchUser = GetByServiceName(ServiceName);
        if(SearchUser != null)
        {
            databaseContext.UserDates.Remove(SearchUser);
            databaseContext.SaveChanges();

        }
    }
}
