using BahamutService.Model;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        
        private IRedisClientsManager psClientManager { get; set; }

        public BahamutPubSubService(IRedisClientsManager psClientManager)
        {
            this.psClientManager = psClientManager;
        }

        public bool RemoveUserDevice(string userId)
        {
            return psClientManager.GetClient().Remove(userId);
        }

        public void RegistUserDevice(string userId,DeviceToken deviceToken,TimeSpan expireTime)
        {
            psClientManager.GetClient().Set(userId, deviceToken, expireTime);
        }

        public DeviceToken GetUserDeviceToken(string userId)
        {
            return psClientManager.GetReadOnlyClient().Get<DeviceToken>(userId);
        }

        public DeviceToken GetUserDeviceToken(string userId, TimeSpan expireTime)
        {
            var client = psClientManager.GetClient();
            if (client.ExpireEntryIn(userId, expireTime))
            {
                return client.Get<DeviceToken>(userId);
            }
            return null;
        }

        public void ExpireUserDeviceToken(string userId,TimeSpan expireTime)
        {
            psClientManager.GetClient().ExpireEntryIn(userId, expireTime);
        }

        public void PublishBahamutUserNotifyMessage(string appUniqueId, BahamutPublishModel message)
        {
            if (string.IsNullOrWhiteSpace(appUniqueId))
            {
                throw new Exception("App Id Is Empty");
            }
            if (string.IsNullOrWhiteSpace(message.ToUser))
            {
                throw new Exception("To User Is Empty");
            }
            if (string.IsNullOrWhiteSpace(message.NotifyType))
            {
                throw new Exception("Notify Type Is Empty");
            }
            Task.Run(() =>
            {
                using (var psClient = psClientManager.GetClient())
                {
                    psClient.PublishMessage(appUniqueId, JsonConvert.SerializeObject(message));

                }
            });
        }
        
        public void UnSubscribe(string channel)
        {
            psClientManager.GetClient().PublishMessage(channel, UnSubscribeMessage);
        }

        public IRedisSubscription CreateSubscription()
        {
            return psClientManager.GetClient().CreateSubscription();
        }

        public BahamutPublishModel DeserializePublishMessage(string message)
        {
            return JsonConvert.DeserializeObject<BahamutPublishModel>(message);
        }

    }
}
