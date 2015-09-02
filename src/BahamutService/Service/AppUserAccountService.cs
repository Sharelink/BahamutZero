using BahamutService.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService
{
    public class AppUserAccountService
    {
        protected BahamutDBContext DBContext { get; private set; }

        public AppUserAccountService(string connectionString)
            :this(new BahamutDBContext(connectionString))
        {
        }

        public AppUserAccountService(BahamutDBContext DBContext)
        {
            this.DBContext = DBContext;
        }

        public async Task<bool> BindAppUserWithBahamutAccount(string appkey, string bahamutAccountId, string appUserId)
        {
            try
            {
                var userDbContext = GetAppUserDBContext(appkey);
                var suc = userDbContext.BindAppUserAccount(bahamutAccountId, appUserId);
                if (suc)
                {
                    await userDbContext.SaveChangesAsync();
                }
                return suc;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
        }

        private AppUserDBContext GetAppUserDBContext(string appkey)
        {
            var apps = from u in DBContext.App where u.Appkey == appkey select u;
            if (apps.Count() > 0)
            {
                dynamic appDoc = apps.First().DocumentModel;
                if (appDoc.AppUserDBConnectionConfig != null)
                {
                    var dbConnectionModel = appDoc.AppUserDBConnectionConfig;
                    string userTableName = dbConnectionModel.TableName;
                    string appUserDbConStr = dbConnectionModel.DBConnectionString;
                    return new AppUserDBContext(appUserDbConStr, userTableName);
                }
                else
                {
                    throw new Exception("App Can't Login");
                }
            }
            throw new Exception("No Such App");
        }

        public AppUserAccount GetAppUser(string appkey, string idValue, string idFieldName)
        {
            var userDbContext = GetAppUserDBContext(appkey);
            var result = userDbContext.GetAppUserAccount(idFieldName, idValue);
            return result;
        }

        public bool IsAppUserIdExists(string appkey,string appUserId)
        {
            try
            {
                GetAccountIdOfAppUserUserId(appkey, appUserId);
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public string GetAppUserIdOfAccountId(string appkey, string accountId)
        {
            var user = GetAppUser(appkey, accountId, "AccountID");
            return user.AccountID.ToString();
        }

        public string GetAccountIdOfAppUserUserId(string appkey, string appUserId)
        {
            var user = GetAppUser(appkey, appUserId, "AppUserID");
            return user.AccountID.ToString();
        }

        public void SaveAllChanges()
        {
            DBContext.SaveChangesAsync();
        }
    }
}
