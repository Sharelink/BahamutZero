using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace BahamutCommon
{
    public class TokenUtil
    {
        public static string GenerateToken(string appkey,string accountId)
        {
            var guid = Guid.NewGuid();
            var code = string.Format("{0}{1}{2}{3}",guid.ToString(), appkey, accountId, DateTime.UtcNow.ToLongTimeString());
            return PasswordHash.Encrypt.SHA1(code);
        }

        public static string GenerateKeyOfToken(string appkey,string uniqueId,string token)
        {
            var strings = string.Format("{0}#{1}#{2}",appkey, uniqueId, token);
            return PasswordHash.Encrypt.MD5(strings);
        }
    }
}
