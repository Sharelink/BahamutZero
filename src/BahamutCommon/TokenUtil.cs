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
            var sha = new DBTek.Crypto.UnixCrypt();
            var guid = Guid.NewGuid();
            return sha.HashString(guid.ToString() + appkey + accountId + DateTime.UtcNow.ToLongTimeString(), DBTek.Crypto.UnixCryptTypes.SHA2_256);
        }

        public static string GenerateKeyOfToken(string appkey,string uniqueId,string token)
        {
            var md = new DBTek.Crypto.MD5_Hsr();
            var strings = string.Format("{0}#{1}#{2}",appkey, uniqueId, token);
            var hash = md.HashString(strings);
            return hash;
        }
    }
}
