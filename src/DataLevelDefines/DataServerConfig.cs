using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

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

        public static ConnectionMultiplexer GenerateRedisConnectionMultiplexer(IConfigurationSection configSection)
        {
            return GenerateRedisConnectionMultiplexer(new DataServerConfigSet(configSection));
        }

        public static ConnectionMultiplexer GenerateRedisConnectionMultiplexer(DataServerConfigSet config)
        {
            var hosts = config.Masters.Union(config.Slavers);
            var password = "";
            var rwHost = hosts.Select(s =>
            {
                var url = s.serverUrl.Replace("redis://", "");
                if (url.Contains("@"))
                {
                    var parts = url.Split(new char[] { '@' });
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        password = parts[0];
                    }
                    return parts[1];
                }
                else
                {
                    return url;
                }
            }).Where(s => !string.IsNullOrWhiteSpace(s));
            var conString = string.Join(",", rwHost);
            if (!string.IsNullOrWhiteSpace(password))
            {
                conString = string.Format("{0},password={1}", conString, password);
            }
            return ConnectionMultiplexer.Connect(conString);
        }
    }

}
