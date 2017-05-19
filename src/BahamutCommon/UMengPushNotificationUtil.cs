using BahamutCommon.Encryption;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UMengTools
{

    public class UMengMessageModel
    {
        public class APS
        {
            public APS()
            {
                badge = 1;
                sound = "default";
            }

            public object alert { get; set; }
            public int content_available { get; set; }
            public int badge { get; set; }
            public string sound { get; set; }
            public string category { get; set; }
        }

        public class APSPayload
        {
            public APS aps { get; set; }
            public string custom { get; set; }
        }

        public class AndroidPayload
        {
            public AndroidPayload()
            {
                display_type = "notification";
            }
            public ABody body { get; set; }
            public object extra { get; set; }
            public string display_type { get; set; }
        }

        public class ABody
        {
            public ABody()
            {
                title = "app_name";
                ticker = "new_msg";
            }

            public string ticker { get; set; }
            public string title { get; set; }
            public string text { get; set; }
            public string after_open { get; set; }
            public string custom { get; set; }
            public int builder_id { get; set; }
        }

        public APSPayload apsPayload { get; set; }
        public AndroidPayload androidPayload { get; set; }

        public string ProductionMode { get; set; }

        public string toMiniJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }

    public class UMengPushNotificationUtil
    {
        public static async Task PushAndroidNotifyToUMessage(string deviceTokens, string appkey, string app_master_secret, UMengMessageModel model)
        {
            var type = deviceTokens.Contains(",") ? "listcast" : "unicast";
            var p = new
            {
                appkey = appkey,
                timestamp = (long)BahamutCommon.DateTimeUtil.UnixTimeSpan.TotalSeconds,
                device_tokens = deviceTokens,
                type = type,
                production_mode = string.IsNullOrWhiteSpace(model.ProductionMode) ? "true" : model.ProductionMode,
                payload = model.androidPayload
            };
            await PushNotifyToUMessage(app_master_secret, p);
        }

        public static async Task PushAPNSNotifyToUMessage(string deviceTokens, string appkey, string app_master_secret, UMengMessageModel model)
        {
            var type = deviceTokens.Contains(",") ? "listcast" : "unicast";
            var p = new
            {
                appkey = appkey,
                timestamp = (long)BahamutCommon.DateTimeUtil.UnixTimeSpan.TotalSeconds,
                device_tokens = deviceTokens,
                type = type,
                production_mode = string.IsNullOrWhiteSpace(model.ProductionMode) ? "true" : model.ProductionMode,
                payload = model.apsPayload

            };
            await PushNotifyToUMessage(app_master_secret, p);
        }

        private static Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings
        {
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.None
        };

        public static async Task PushNotifyToUMessage(string app_master_secret, object msgParams)
        {
            var method = "POST";
            var url = "http://msg.umeng.com/api/send";
            var post_body = Newtonsoft.Json.JsonConvert.SerializeObject(msgParams, JsonSerializerSettings)
            .Replace("loc_key", "loc-key").Replace("content_available", "content-available");
            var sign = MD5.ComputeMD5Hash(string.Format("{0}{1}{2}{3}", method, url, post_body, app_master_secret));
            var client = new HttpClient();
            var uri = new Uri(string.Format("{0}?sign={1}", url, sign));
            var msg = await client.PostAsync(uri, new StringContent(post_body, System.Text.Encoding.UTF8, "application/json"));
            var result = await msg.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(result) || !result.Contains("SUCCESS"))
            {
                LogManager.GetLogger("Warn").Info("UMSG:{0}", result);
            }
            else
            {
                LogManager.GetLogger("Info").Info("UMSG:{0}", result);
            }

        }
    }
}
