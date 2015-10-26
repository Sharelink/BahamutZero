using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CSharp;
using DBTek.Crypto;

namespace BahamutCommon
{
    public class TokenUtil
    {
        private static SHA1_Hsr sha1 = new SHA1_Hsr();
        private static MD5_Hsr md5 = new MD5_Hsr();
        public static string GenerateToken(string appkey,string accountId)
        {
            var guid = Guid.NewGuid();
            var code = string.Format("{0}{1}{2}{3}",guid.ToString(), appkey, accountId, DateTime.UtcNow.ToLongTimeString());
            return sha1.HashString(code);
        }

        public static string GenerateKeyOfToken(string appkey,string uniqueId,string token)
        {
            var code = string.Format("{0}#{1}#{2}",appkey, uniqueId, token);
            return md5.HashString(code);
        }
    }
}
