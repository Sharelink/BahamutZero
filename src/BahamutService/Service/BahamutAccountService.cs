using BahamutService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BahamutService
{
    public class BahamutAccountService
    {
        protected BahamutDBContext DBContext { get { return new BahamutDBContext(connectionString); } }
        public string connectionString { get; private set; }
        
        public BahamutAccountService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string AddAccount(Account newBahamutAccount)
        {
            var db = DBContext;
            db.Account.Add(newBahamutAccount);
            db.SaveChanges();
            return newBahamutAccount.AccountID.ToString();
        }

        public bool ChangePassword(string accountId,string oldPassword, string newPassword)
        {
            try
            {
                var db = DBContext;
                var account = db.Account.Single(a => a.AccountID.ToString() == accountId && a.Password == oldPassword);
                account.Password = newPassword;
                return db.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ChangeAccountEmail(string accountId,string newEmail)
        {
            var db = DBContext;
            var account = db.Account.Single(a => a.AccountID.ToString() == accountId);
            account.Email = newEmail;
            return db.SaveChanges() > 0;
        }

        public bool ChangeAccountMobile(string accountId,string newMobile)
        {
            var db = DBContext;
            var account = db.Account.Single(a => a.AccountID.ToString() == accountId);
            account.Mobile = newMobile;
            return db.SaveChanges() > 0;
        }

        public bool ChangeAccountName(string accountId,string newName)
        {
            var db = DBContext;
            var account = db.Account.Single(a => a.AccountID.ToString() == accountId);
            account.AccountName = newName;
            return db.SaveChanges() > 0;
        }

        public bool ChangeAccountBirthday(string accountId,DateTime newBirth)
        {
            var db = DBContext;
            var account = db.Account.Single(a => a.AccountID.ToString() == accountId);
            return db.SaveChanges() > 0;
        }

        public bool ChangeName(string accountId,string newName)
        {
            var db = DBContext;
            var account = db.Account.Single(a => a.AccountID.ToString() == accountId);
            account.Name = newName;
            return db.SaveChanges() > 0;
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
        
    }

    public static class GetBahamutAccountServiceExtension
    {
        public static BahamutAccountService GetBahamutAccountService(this IServiceProvider provider)
        {
            return provider.GetService<BahamutAccountService>();
        }
    }

}
