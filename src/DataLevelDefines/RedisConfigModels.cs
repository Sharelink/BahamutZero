using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLevelDefines
{
    public interface IRedisServerConfig
    {
        string Host { get; set; }
        int Port { get; set; }
        string Password { set; get; }
        long Db { get; set; }
    }

    public class RedisServerConfig : IRedisServerConfig
    {
        public RedisServerConfig()
        {

        }

        public RedisServerConfig(string host,int port,string password,long db)
        {
            this.Db = db;
            this.Host = host;
            this.Password = password;
            this.Port = port;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { set; get; }
        public long Db { get; set; }
    }
}
