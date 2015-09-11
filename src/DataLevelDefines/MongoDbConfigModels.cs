using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLevelDefines
{
    public interface IMongoDbServerConfig
    {
        string Url { get; set; }
    }

    public class MongoDbServerConfig: IMongoDbServerConfig
    {
        public string Url { get; set; }
    }
}
