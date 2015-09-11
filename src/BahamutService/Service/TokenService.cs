using BahamutCommon;
using BahamutService.Model;
using DataLevelDefines;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService
{

    public class TokenService
    {
        protected RedisClient Client { get; private set; }

        public TokenService(IRedisServerConfig ServerConfig)
            : this(ServerConfig.Host, ServerConfig.Port, ServerConfig.Password, ServerConfig.Db)
        { }

        public TokenService(string host,int port,string password = null, long db = 0):
            this(new RedisClient(new RedisEndpoint(host, port, password, db)))
        {
        }

        public TokenService(RedisClient Client)
        {
            this.Client = Client;
        }

        async public Task<AccountSessionData> AllocateAccessToken(AccountSessionData sessionData)
        {
            return await Task.Run(() =>
            {
                sessionData.AccessToken = TokenUtil.GenerateToken(sessionData.Appkey, sessionData.AccountId);
                double timeLimitMinutes = 10;
                var timeExpiresIn = TimeSpan.FromMinutes(timeLimitMinutes);
                var key = TokenUtil.GenerateKeyOfToken(sessionData.Appkey, sessionData.AccountId, sessionData.AccessToken);
                var redisSessionData = Client.As<AccountSessionData>();
                redisSessionData.SetEntry(key, sessionData, timeExpiresIn);
                return sessionData;
            });
        }

        public bool ReleaseAppToken(string appkey, string userId, string appToken)
        {
            var key = TokenUtil.GenerateKeyOfToken(appkey, userId, appToken);
            var redisSessionData = Client.As<AccountSessionData>();
            return redisSessionData.RemoveEntry(key);
        }

        public AccountSessionData ValidateToGetSessionData(string appkey, string accountId, string AccessToken)
        {
            var sessionDataRedis = Client.As<AccountSessionData>();
            var key = TokenUtil.GenerateKeyOfToken(appkey, accountId, AccessToken);
            return sessionDataRedis[key];
        }

        public AccessTokenValidateResult ValidateAccessToken(string appkey, string accountId, string AccessToken, string UserId)
        {
            var sessionDataRedis = Client.As<AccountSessionData>();
            var AccountSessionData = ValidateToGetSessionData(appkey, accountId, AccessToken);
            var key = TokenUtil.GenerateKeyOfToken(appkey, accountId, AccessToken);
            if (AccountSessionData == null)
            {
                return new AccessTokenValidateResult() { Message = "Validate Failed" };
            }
            else if (Client.Remove(key))
            {
                try
                {
                    double timeLimitDays = 7;
                    AccountSessionData.AccessToken = null;
                    AccountSessionData.UserId = UserId;
                    AccountSessionData.AppToken = TokenUtil.GenerateToken(appkey, AccountSessionData.UserId);
                    key = TokenUtil.GenerateKeyOfToken(appkey, AccountSessionData.UserId, AccountSessionData.AppToken);
                    sessionDataRedis.SetEntry(key, AccountSessionData, TimeSpan.FromDays(timeLimitDays));

                    return new AccessTokenValidateResult()
                    {
                        UserSessionData = AccountSessionData
                    };
                }
                catch (Exception ex)
                {
                    return new AccessTokenValidateResult() { Message = ex.Message };
                }
            }
            else
            {
                return new AccessTokenValidateResult() { Message = "Server Error" };
            }
        }

        async public Task<AccountSessionData> ValidateAppToken(string appkey, string userId, string AppToken)
        {
            return await Task.Run(() =>
            {
                var sessionDataRedis = Client.As<AccountSessionData>();
                var key = TokenUtil.GenerateKeyOfToken(appkey, userId, AppToken);
                var AccountSessionData = sessionDataRedis[key];
                if (AccountSessionData == null)
                {
                    return null;
                }
                else
                {
                    double timeLimitDays = 7;
                    sessionDataRedis.ExpireEntryIn(key, TimeSpan.FromDays(timeLimitDays));
                    return AccountSessionData;
                }
            });
        }
    }
}
