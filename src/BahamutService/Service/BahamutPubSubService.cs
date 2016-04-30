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
    }

    public class BahamutPubSubService
    {
        public static readonly string UnSubscribeMessage = "UnSubscribe";
        

        private IRedisClientsManager psClientManager { get; set; }

        public BahamutPubSubService(IRedisClientsManager psClientManager)
        {
            this.psClientManager = psClientManager;
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
