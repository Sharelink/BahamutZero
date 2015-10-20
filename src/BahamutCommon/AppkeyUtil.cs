using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBTek.Crypto;

namespace BahamutCommon
{
    public class AppkeyUtil
    {
        /// <summary>
        /// Generate a string length less than 128
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static string GenerateAppkey(string appName)
        {
            var guid = Guid.NewGuid();
            var sha = new SHA1_Hsr();
            var code = string.Format("{0}_{1}_{2}", guid.ToString(), appName,DateTime.UtcNow.Ticks);
            return sha.HashString(code);
        }
    }
}
