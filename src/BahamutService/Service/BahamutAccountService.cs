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

        public bool ChangePassword(string accountId,string oldPassword, string newPassword)
        {
            return true;
        }

        public bool ChangeAccountEmail(string accountId,string newEmail)
        {
            return true;
        }

        public bool ChangeAccountMobile(string accountId,string newMobile)
        {
            return true;
        }

        public bool ChangeAccountName(string accountId,string newName)
        {
            return true;
        }

        public bool ChangeAccountBirthday(string accountId,DateTime newBirth)
        {
            return true;
        }

        public bool ChangeName(string accountId,string newName)
        {
            return true;
        }

        public Account GetAccount(string accountId)
        {
            return new Account() {  };
        }

        public void SaveAllChanges()
        {
            DBContext.SaveChangesAsync();
        }
    }

}
