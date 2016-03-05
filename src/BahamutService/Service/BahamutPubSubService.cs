using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService.Service
{
    public class BahamutUserAppNotifyMessage
    {
        public string NotificationType { get; set; }
        public string DeserializableMessage { get; set; }
        public DateTime PublishDateTime { get; set; }
        public string ExtraInfo { get; set; }
    }

    public class BahamutPublishModel
    {
        public string ToUser { get; set; }
        public string NotifyType { get; set; }
        public string Info { get; set; }
    }

    public class BahamutPubSubService
    {
        public static readonly string UnSubscribeMessage = "UnSubscribe";
        private IRedisClientsManager mcClientManager;

        private IRedisClientsManager psClientManager { get; set; }

        public BahamutPubSubService(IRedisClientsManager psClientManager, IRedisClientsManager mcClientManager)
        {
            this.mcClientManager = mcClientManager;
            this.psClientManager = psClientManager;
        }

        public void PublishBahamutUserNotifyMessage(string appUniqueId, string toUserId, BahamutUserAppNotifyMessage message)
        {
            Task.Run(() =>
            {
                using (var psClient = psClientManager.GetClient())
                {
                    using (var msgClient = mcClientManager.GetClient())
                    {
                        var client = msgClient.As<BahamutUserAppNotifyMessage>();

                        var key = GenerateMessageListKey(appUniqueId, message.NotificationType, toUserId);
                        var list = client.Lists[key];
                        list.Append(message);
                        var notifyMsg = new BahamutPublishModel()
                        {
                            ToUser = toUserId,
                            NotifyType = message.NotificationType,
                            Info = message.ExtraInfo
                        };
                        psClient.PublishMessage(appUniqueId, JsonConvert.SerializeObject(notifyMsg));

                    }
                }
            });
        }

        public void PublishBahamutUserNotifyMessages(string appUniqueId, string toUserId, IEnumerable<BahamutUserAppNotifyMessage> messages)
        {
            Task.Run(() =>
            {
                using (var psClient = psClientManager.GetClient())
                {
                    using (var msgClient = mcClientManager.GetClient())
                    {
                        var client = msgClient.As<BahamutUserAppNotifyMessage>();
                        foreach (var msg in messages)
                        {
                            var key = GenerateMessageListKey(appUniqueId, msg.NotificationType, toUserId);
                            var list = client.Lists[key];
                            list.Append(msg);
                            var notifyMsg = new BahamutPublishModel()
                            {
                                ToUser = toUserId,
                                NotifyType = msg.NotificationType,
                                Info = msg.ExtraInfo
                            };
                            psClient.PublishMessage(toUserId, JsonConvert.SerializeObject(notifyMsg));
                        }
                    }
                }
            });
        }

        private static string GenerateMessageListKey(string appUniqueId, string notifyType,string userId)
        {
            return string.Format("{0}:{1}:{2}", appUniqueId, notifyType, userId);
        }

        public void UnSubscribe(string channel)
        {
            psClientManager.GetClient().PublishMessage(channel, UnSubscribeMessage);
        }

        public IRedisSubscription CreateSubscription()
        {
            return psClientManager.GetClient().CreateSubscription();
        }

        public async Task<IEnumerable<BahamutUserAppNotifyMessage>> GetBahamutUserNotifyMessages(string appUniqueId, string userId, string notificationType)
        {
            return await Task.Run(() =>
            {
                using (var psClient = psClientManager.GetClient())
                {
                    using (var msgClient = mcClientManager.GetClient())
                    {
                        var client = msgClient.As<BahamutUserAppNotifyMessage>();
                        var key = GenerateMessageListKey(appUniqueId, notificationType, userId);
                        return client.Lists[key].Where(m => m.NotificationType == notificationType);
                    }
                }
            });
        }

        public void ClearBahamutUserNotifyMessages(string appUniqueId, string userId, string notificationType)
        {
            Task.Run(() =>
            {
                using (var psClient = psClientManager.GetClient())
                {
                    using (var msgClient = mcClientManager.GetClient())
                    {
                        var client = msgClient.As<BahamutUserAppNotifyMessage>();
                        var key = GenerateMessageListKey(appUniqueId, notificationType, userId);
                        var list = client.Lists[key];
                        if (list == null)
                        {
                            return;
                        }
                        client.RemoveAllFromList(list);
                    }
                }
            });
        }

        public BahamutPublishModel DeserializePublishMessage(string message)
        {
            return JsonConvert.DeserializeObject<BahamutPublishModel>(message);
        }

    }
}
