using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
namespace BahamutService.Model
{
    
    
    public class BahamutDBContext : DbContext
    {
        public string ConnectionString { get; private set; }
        public virtual DbSet<Account> Account { get; set; }
        public BahamutDBContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySQL(this.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>()
                .Property(e => e.AccountName);

            modelBuilder.Entity<Account>()
                .Property(e => e.Email);

            modelBuilder.Entity<Account>()
                .Property(e => e.Mobile);

            modelBuilder.Entity<Account>()
                .Property(e => e.Name);

            modelBuilder.Entity<Account>()
                .Property(e => e.Password);

            modelBuilder.Entity<Account>()
                .Property(e => e.Extra);
        }
    }    
}
