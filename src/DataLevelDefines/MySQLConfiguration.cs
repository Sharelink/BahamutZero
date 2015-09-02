using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlDefines
{
    /// <summary>
    /// ef6 in asp.net 5 not use the app.config file to config the ef
    /// </summary>
    public class MySqlDbConfigurationTypeAttribute : DbConfigurationTypeAttribute
    {
        public MySqlDbConfigurationTypeAttribute():
            base(typeof(MySqlEFConfiguration))
        { }
    }

}
