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
            DBContext.SaveChanges();
            return newBahamutAccount.AccountID.ToString();
        }

        public bool ChangePassword(string accountId,string oldPassword, string newPassword)
        {
            var account = DBContext.Account.Single(a => a.AccountID.ToString() == accountId);
            if (account.Password == oldPassword)
            {
                account.Password = newPassword;
                return DBContext.SaveChanges() > 0;
            }
            return false;
        }

        public bool ChangeAccountEmail(string accountId,string newEmail)
        {
            var account = DBContext.Account.Single(a => a.AccountID.ToString() == accountId);
            account.Email = newEmail;
            return DBContext.SaveChanges() > 0;
        }

        public bool ChangeAccountMobile(string accountId,string newMobile)
        {
            var account = DBContext.Account.Single(a => a.AccountID.ToString() == accountId);
            account.Mobile = newMobile;
            return DBContext.SaveChanges() > 0;
        }

        public bool ChangeAccountName(string accountId,string newName)
        {
            var account = DBContext.Account.Single(a => a.AccountID.ToString() == accountId);
            account.AccountName = newName;
            return DBContext.SaveChanges() > 0;
        }

        public bool ChangeAccountBirthday(string accountId,DateTime newBirth)
        {
            var account = DBContext.Account.Single(a => a.AccountID.ToString() == accountId);
            return DBContext.SaveChanges() > 0;
        }

        public bool ChangeName(string accountId,string newName)
        {
            var account = DBContext.Account.Single(a => a.AccountID.ToString() == accountId);
            account.Name = newName;
            return DBContext.SaveChanges() > 0;
        }

        public bool AccountExists(string accountName)
        {
            try
            {
                var accounts = from a in DBContext.Account where a.AccountName == accountName select a.AccountID;
                return accounts.Count() > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Account GetAccount(string accountId)
        {
            var account = DBContext.Account.Single(a => a.AccountID.ToString() == accountId);
            return account;
        }

        public void SaveAllChanges()
        {
            DBContext.SaveChangesAsync();
        }
    }

}
