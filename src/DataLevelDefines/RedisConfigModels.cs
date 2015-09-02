using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLevelDefines
{
    public interface ITokenRedisServerConfig
    {
        string Host { get; set; }
        int Port { get; set; }
        string Password { set; get; }
        long Db { get; set; }
    }

    public class TokenRedisServerConfig : ITokenRedisServerConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { set; get; }
        public long Db { get; set; }
    }
}
