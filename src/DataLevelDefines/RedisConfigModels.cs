using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLevelDefines
{
    public interface IRedisServerConfig
    {
        string Url { get; set; }
    }

    public class RedisServerConfig : IRedisServerConfig
    {
        public string Url { get; set; }
    }
}
