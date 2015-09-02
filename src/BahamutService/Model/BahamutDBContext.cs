using MySqlDefines;
using System;
using System.Data.Entity;
using System.Linq;

namespace BahamutService.Model
{
    [MySqlDbConfigurationType]
    public class BahamutDBContext : DbContext
    {
        public BahamutDBContext(string connectionString)
            : base(connectionString)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<App> App { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.AccountName)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Extra)
                .IsUnicode(false);

            modelBuilder.Entity<App>()
                .Property(e => e.Appkey)
                .IsUnicode(false);

            modelBuilder.Entity<App>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<App>()
                .Property(e => e.Document)
                .IsUnicode(false);

            modelBuilder.Entity<App>()
                .Property(e => e.Description)
                .IsUnicode(false);
        }
    }

    
}