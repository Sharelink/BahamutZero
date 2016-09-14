using BahamutCommon;
using BahamutService.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace BahamutService
{

    public class TokenService
    {
        public TimeSpan AppTokenExipreTime { get; set; }
        public TimeSpan AccessTokenExipreTime { get; set; }

        private ConnectionMultiplexer redis;

        public TokenService(ConnectionMultiplexer redis)
        {
            AppTokenExipreTime = TimeSpan.FromDays(14);
            AccessTokenExipreTime = TimeSpan.FromMinutes(3);
            this.redis = redis;
        }

        async public Task<AccountSessionData> AllocateAccessTokenAsync(AccountSessionData sessionData)
        {
            var db = redis.GetDatabase();
            sessionData.AccessToken = TokenUtil.GenerateToken(sessionData.Appkey, sessionData.AccountId);
            var key = TokenUtil.GenerateKeyOfToken(sessionData.Appkey, sessionData.AccountId, sessionData.AccessToken);
            var suc = await db.StringSetAsync(key, sessionData.ToJson());
            if (suc)
            {
                return sessionData;
            }
            throw new Exception("Allocate AccessToken Error");
        }

        public async Task<bool> ReleaseAppTokenAsync(string appkey, string userId, string appToken)
        {
            var db = redis.GetDatabase();
            var key = TokenUtil.GenerateKeyOfToken(appkey, userId, appToken);
            return await db.KeyDeleteAsync(key);
        }

        public async Task<AccountSessionData> ValidateToGetSessionDataAsync(string appkey, string accountId, string AccessToken)
        {
            var key = TokenUtil.GenerateKeyOfToken(appkey, accountId, AccessToken);
            var json = await redis.GetDatabase().StringGetAsync(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<AccountSessionData>(json);
        }

        public async Task<AccessTokenValidateResult> ValidateAccessTokenAsync(string appkey, string accountId, string AccessToken, string UserId)
        {
            var db = redis.GetDatabase();
            var key = TokenUtil.GenerateKeyOfToken(appkey, accountId, AccessToken);
            var dataJson = await db.StringGetAsync(key);
            var AccountSessionData = string.IsNullOrWhiteSpace(dataJson) ? null : JsonConvert.DeserializeObject<AccountSessionData>(dataJson);
            if (AccountSessionData == null)
            {
                return new AccessTokenValidateResult() { Message = "Validate Failed" };
            }
            else
            {
                try
                {
                    AccountSessionData.AccessToken = null;
                    AccountSessionData.UserId = UserId;
                    AccountSessionData.AppToken = TokenUtil.GenerateToken(appkey, AccountSessionData.UserId);
                    key = TokenUtil.GenerateKeyOfToken(appkey, AccountSessionData.UserId, AccountSessionData.AppToken);
                    var suc = await db.StringSetAsync(key, AccountSessionData.ToJson());
                    if (suc)
                    {
                        return new AccessTokenValidateResult()
                        {
                            UserSessionData = AccountSessionData
                        };
                    }
                    else
                    {
                        throw new Exception("Allocate App Token Failed");
                    }
                }
                catch (Exception ex)
                {
                    return new AccessTokenValidateResult() { Message = ex.Message };
                }
            }
        }


        async public Task<AccountSessionData> ValidateAppTokenAsync(string appkey, string userId, string AppToken)
        {

            var key = TokenUtil.GenerateKeyOfToken(appkey, userId, AppToken);
            var db = redis.GetDatabase();
            var res = await db.StringGetWithExpiryAsync(key, CommandFlags.PreferSlave);
            if (res.Expiry.HasValue && res.Expiry.Value.TotalSeconds > 0)
            {
                if (res.Expiry.Value.TotalSeconds < AppTokenExipreTime.TotalSeconds * 0.1)
                {
                    await db.KeyExpireAsync(key, AppTokenExipreTime);
                }
            }
            if (!string.IsNullOrWhiteSpace(res.Value))
            {
                return JsonConvert.DeserializeObject<AccountSessionData>(res.Value);
            }
            return null;
            
        }

        public async Task<bool> SetUserSessionDataAsync(AccountSessionData userSessionData)
        {
            var key = TokenUtil.GenerateKeyOfToken(userSessionData.Appkey, userSessionData.UserId, userSessionData.AppToken);
            return await redis.GetDatabase().StringSetAsync(key, userSessionData.ToJson(), AppTokenExipreTime);
        }
    }

    public static class GetTokenServiceExtension
    {
        public static TokenService GetTokenService(this IServiceProvider provider)
        {
            return provider.GetService<TokenService>();
        }
    }
}
