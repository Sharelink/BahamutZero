using BahamutService.Model;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace BahamutService.Service
{

    public class BahamutPublishModel
    {
        public string ToUser { get; set; }
        public string NotifyType { get; set; }
        public string Info { get; set; }
        public string NotifyInfo { get; set; }
        public string CustomCmd { get; set; }
    }

    public class BahamutPubSubService
    {
        public static readonly string UnSubscribeMessage = "UnSubscribe";
        private ConnectionMultiplexer pubsubRedis { get; set; }

        public BahamutPubSubService(ConnectionMultiplexer redis)
        {
            this.pubsubRedis = redis;
        }

        public async Task<bool> RemoveUserDeviceAsync(string userId)
        {
            return await pubsubRedis.GetDatabase().KeyDeleteAsync(userId);
        }

        public async Task<bool> RegistUserDeviceAsync(string userId, DeviceToken deviceToken, TimeSpan expireTime)
        {
            var dt = JsonConvert.SerializeObject(deviceToken, Formatting.None);
            return await pubsubRedis.GetDatabase().StringSetAsync(userId, dt, expireTime);
        }

        public async Task<Tuple<DeviceToken,TimeSpan>> GetUserDeviceTokenWithExpiryAsync(string userId)
        {
            var dt = await pubsubRedis.GetDatabase().StringGetWithExpiryAsync(userId, CommandFlags.PreferSlave);
            if (!string.IsNullOrWhiteSpace(dt.Value) && dt.Expiry.HasValue)
            {
                return new Tuple<DeviceToken, TimeSpan>(JsonConvert.DeserializeObject<DeviceToken>(dt.Value), dt.Expiry.Value);
            }
            return null;   
        }

        public async Task<DeviceToken> GetUserDeviceTokenAsync(string userId)
        {
            var dt = await pubsubRedis.GetDatabase().StringGetAsync(userId, CommandFlags.PreferSlave);
            return JsonToDeviceToken(dt);
        }

        public async Task<IEnumerable<DeviceToken>> GetUserDeviceTokensAsync(IEnumerable<string> userIds)
        {
            var keys = new RedisKey[userIds.Count()];
            for (int i = 0; i < userIds.Count(); i++)
            {
                keys[i] = userIds.ElementAt(i);
            }
            var dtJsons = await pubsubRedis.GetDatabase().StringGetAsync(keys.ToArray(), CommandFlags.PreferSlave);
            return from item in dtJsons select JsonToDeviceToken(item);
        }

        static private DeviceToken JsonToDeviceToken(RedisValue item)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<DeviceToken>(item);
                return result;
            }
            catch (System.Exception)
            {
                return new DeviceToken();
            }
            
        }

        public async Task<TimeSpan> GetDeviceTokenTimeToLiveAsync(string userId)
        {
            var ts = await pubsubRedis.GetDatabase().KeyTimeToLiveAsync(userId);
            return ts.HasValue ? ts.Value : TimeSpan.FromSeconds(0);
        }

        public async Task<DeviceToken> GetUserDeviceTokenAsync(string userId, TimeSpan expireTime)
        {
            var db = pubsubRedis.GetDatabase();
            await db.KeyExpireAsync(userId, expireTime);
            var dt = await db.StringGetAsync(userId, CommandFlags.PreferSlave);
            if (!string.IsNullOrWhiteSpace(dt))
            {
                return JsonConvert.DeserializeObject<DeviceToken>(dt);
            }
            return null;
        }

        private static string RemoveChannelPrefix(string channelWithPrefix)
        {
            return channelWithPrefix.Replace("btpbs_", "");
        }

        private static string AddChannelWithPrefix(string channel)
        {
            return string.Format("btpbs_{0}", channel);
        }

        public async Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> action)
        {
            await pubsubRedis.GetSubscriber().SubscribeAsync(AddChannelWithPrefix(channel), (c, v) =>
            {
                action(RemoveChannelPrefix(c), v);
            });
        }

        public async Task<bool> ExpireUserDeviceTokenAsync(string userId, TimeSpan expireTime)
        {
            return await pubsubRedis.GetDatabase().KeyExpireAsync(userId, expireTime);
        }

        public void PublishBahamutUserNotifyMessage(string appChannel, BahamutPublishModel message)
        {
            if (string.IsNullOrWhiteSpace(appChannel))
            {
                throw new Exception("App Channel Is Empty");
            }
            if (string.IsNullOrWhiteSpace(message.ToUser))
            {
                throw new Exception("To User Is Empty");
            }
            if (string.IsNullOrWhiteSpace(message.NotifyType))
            {
                throw new Exception("Notify Type Is Empty");
            }
            Task.Run(async () =>
            {
                await pubsubRedis.GetDatabase().PublishAsync(AddChannelWithPrefix(appChannel), JsonConvert.SerializeObject(message, Formatting.None));
            });
        }
        
        public void UnSubscribe(string channel)
        {
            Task.Run(async () =>
            {
                await pubsubRedis.GetDatabase().PublishAsync(AddChannelWithPrefix(channel), UnSubscribeMessage);
            });
        }

        /*
        public ISubscriber CreateSubscription()
        {
            return pubsubRedis.GetSubscriber();
        }
        */

        public BahamutPublishModel DeserializePublishMessage(string message)
        {
            return JsonConvert.DeserializeObject<BahamutPublishModel>(message);
        }

    }

    public static class GetBahamutPubSubServiceExtension
    {
        public static BahamutPubSubService GetBahamutPubSubService(this IServiceProvider provider)
        {
            return provider.GetService<BahamutPubSubService>();
        }
    }
}
