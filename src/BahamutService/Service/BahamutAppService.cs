using BahamutService.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService
{
    public class BahamutAppService
    {
        protected BahamutDBContext DBContext { get; private set; }
        public DbSet<App> App { get { return DBContext.App; } }

        public BahamutAppService(string connectionString)
            :this(new BahamutDBContext(connectionString))
        {
        }

        public BahamutAppService(BahamutDBContext DBContext)
        {
            this.DBContext = DBContext;
        }

        public App AddBahamutApp(App newApp)
        {
            var addedApp = App.Add(newApp);
            return addedApp;
        }

        public App FindAppByAppkey(string appkey)
        {
            var res = from ap in App where ap.Appkey == appkey select ap;
            if (res.Count() > 0)
            {
                return res.First();
            }
            else
            {
                throw new Exception(string.Format("No Such App With Appkey:{0}", appkey));
            }
        }

        public App FindAppByAppname(string appname)
        {
            var res = from ap in App where ap.Name == appname select ap;
            if (res.Count() > 0)
            {
                return res.First();
            }
            else
            {
                throw new Exception(string.Format("No Such App With Appname:{0}", appname));
            }
        }

        public void SaveAllChanges()
        {
            DBContext.SaveChangesAsync();
        }
    }
}
