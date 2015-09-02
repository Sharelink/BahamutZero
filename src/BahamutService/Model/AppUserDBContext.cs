using MySql.Data.MySqlClient;
using MySqlDefines;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService.Model
{
    public class AppUserAccount
    {
        public long AccountID { get; set; }

        public long AppUserID { get; set; }

        public DateTime CreateTime { get; set; }
    }

    [MySqlDbConfigurationType]
    public class AppUserDBContext : DbContext
    {
        public string UserTableName { get; set; }
        public AppUserDBContext(string connectionString,string userTableName)
            : base(connectionString)
        {
            UserTableName = userTableName;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }

        public AppUserAccount GetAppUserAccount(string idFieldName,string idValue)
        {
            var parameters = new object[] { new MySqlParameter(string.Format("@{0}", idFieldName), idValue) };
            var sql = string.Format("SELECT * FROM {0} WHERE {1} = @{2}", UserTableName, idFieldName, idFieldName);
            var result = Database.SqlQuery<AppUserAccount>(sql, parameters);
            if (result.Count() > 0)
            {
                return result.First();
            }
            else
            {
                throw new NullReferenceException(string.Format("No Such {0} = {1} In Table {2}", idFieldName, idValue, UserTableName));
            }
        }

        public bool BindAppUserAccount(string accountId, string appUserId)
        {
            var parameters = new object[]
                {
                    new MySqlParameter("@AccountId", accountId),
                    new MySqlParameter("@AppUserID", appUserId)
                };
            var sql = string.Format("INSERT INTO {0}(AccountID,AppUserID) VALUES(@AccountId,@AppUserId)", UserTableName);

            var result = Database.ExecuteSqlCommand(sql, parameters);
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
