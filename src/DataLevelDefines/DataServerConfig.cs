using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;

namespace DataLevelDefines
{
    public class DataServerConfig
    {
        public string serverUrl { get; set; }
    }

    public class DataServerConfigSet
    {
        public int MaxPoolSize { get; set; }
        public int MinPoolSize { get; set; }
        public IEnumerable<DataServerConfig> Masters{ get; set; }
        public IEnumerable<DataServerConfig> Slavers { get; set; }
        public DataServerConfigSet()
        {

        }

        public DataServerConfigSet(IConfigurationSection section)
        {
            MaxPoolSize = int.Parse(section["maxPoolSize"]);
            MinPoolSize = int.Parse(section["minPoolSize"]);
            Masters = from s in section.GetSection("masters").GetChildren()
                      select new DataServerConfig
                      {
                          serverUrl = s["url"]
                      };

            Slavers = from s in section.GetSection("slaves").GetChildren()
                      select new DataServerConfig
                      {
                          serverUrl = s["url"]
                      };
        }

    }

    public class DBClientManagerBuilder
    {
        public static MongoDB.Driver.IMongoClient GeneratePoolMongodbClient(IConfigurationSection configSection)
        {
            return GeneratePoolMongodbClient(new DataServerConfigSet(configSection));
        }

        public static MongoDB.Driver.IMongoClient GeneratePoolMongodbClient(DataServerConfigSet configSet)
        {
            var settings = new MongoDB.Driver.MongoClientSettings()
            {
                MaxConnectionPoolSize = configSet.MaxPoolSize,
                MinConnectionPoolSize = configSet.MinPoolSize,
                Server = MongoDB.Driver.MongoServerAddress.Parse(configSet.Masters.First().serverUrl.Replace("mongodb://",""))
            };
            var client = new MongoDB.Driver.MongoClient(settings);
            return client;
        }

        public static PooledRedisClientManager GenerateRedisClientManager(IConfigurationSection configSection)
        {
            return GenerateRedisClientManager(new DataServerConfigSet(configSection));
        }

        public static PooledRedisClientManager GenerateRedisClientManager(DataServerConfigSet configSet)
        {
            var settings = new RedisClientManagerConfig()
            {
                MaxReadPoolSize = configSet.MaxPoolSize,
                MaxWritePoolSize = configSet.MinPoolSize,
            };
            var rwHost = configSet.Masters.Select(s => { return s.serverUrl.Replace("redis://", ""); });
            var rHost = configSet.Slavers.Select(s => { return s.serverUrl.Replace("redis://", ""); });
            var mgr = new PooledRedisClientManager(rwHost, rHost, settings);
            return mgr;
        }
    }

}
