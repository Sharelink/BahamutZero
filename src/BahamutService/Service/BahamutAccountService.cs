using BahamutService.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService
{
    public class BahamutAccountService
    {
        protected BahamutDBContext DBContext { get; private set; }
        public DbSet<Account> Account {get { return DBContext.Account; } }

        public BahamutAccountService(string connectionString)
            :this(new BahamutDBContext(connectionString))
        {
        }

        public BahamutAccountService(BahamutDBContext DBContext)
        {
            this.DBContext = DBContext;
        }

        public string AddAccount(Account newBahamutAccount)
        {
            DBContext.Account.Add(newBahamutAccount);
            return newBahamutAccount.AccountID.ToString();
        }

        public void SaveAllChanges()
        {
            DBContext.SaveChangesAsync();
        }
    }
}
