using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutCommon
{
    public class TokenUtil
    {
        public static string GenerateToken(string appkey,string accountId)
        {
            var sha = new DBTek.Crypto.UnixCrypt();
            var guid = Guid.NewGuid();
            return sha.HashString(guid.ToString() + appkey + accountId + DateTime.Now.ToLongTimeString(), DBTek.Crypto.UnixCryptTypes.SHA2_256);
        }

        public static string GenerateKeyOfToken(string appkey,string uniqueId,string token)
        {
            return string.Format("{0}:{1}_{2}", appkey, uniqueId, token);
        }
    }
}
