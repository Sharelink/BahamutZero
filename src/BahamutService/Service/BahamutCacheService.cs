using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutService.Service
{
    public class BahamutCacheModel
    {
        public string AppUniqueId { get; set; }
        public string Type { get; set; }
        public string UniqueId { get; set; }
        public string DeserializableString { get; set; }
        public DateTime CacheDateTime { get; set; }
        public string ExtraInfo { get; set; }

        public string GetCacheModelListKey()
        {
            return GenerateCacheModelListKey(AppUniqueId, Type, UniqueId);
        }

        public static string GenerateCacheModelListKey(string appUniqueId, string modelType, string uniqueId)
        {
            return string.Format("{0}:{1}:{2}", appUniqueId, modelType, uniqueId);
        }

    }

    public class BahamutCacheService
    {
        private IRedisClientsManager mcClientManager;
        public BahamutCacheService(IRedisClientsManager mcClientManager)
        {
            this.mcClientManager = mcClientManager;
        }

        public void PushCacheModelToList(BahamutCacheModel model)
        {
            Task.Run(() =>
            {
                using (var msgClient = mcClientManager.GetClient())
                {
                    var client = msgClient.As<BahamutCacheModel>();

                    var key = model.GetCacheModelListKey();
                    var list = client.Lists[key];
                    list.Append(model);

                }
            });
        }


        public void ClearCacheModels(string appUniqueId, string modelType, string uniqueId)
        {
            Task.Run(() =>
            {
                using (var msgClient = mcClientManager.GetClient())
                {
                    var client = msgClient.As<BahamutCacheModel>();
                    var key = BahamutCacheModel.GenerateCacheModelListKey(appUniqueId, modelType, uniqueId);
                    var list = client.Lists[key];
                    if (list == null)
                    {
                        return;
                    }
                    client.RemoveAllFromList(list);
                }
            });
        }

        public async Task<IEnumerable<BahamutCacheModel>> GetCacheModels(string appUniqueId, string modelType, string uniqueId)
        {
            return await Task.Run(() =>
            {
                using (var msgClient = mcClientManager.GetClient())
                {
                    var client = msgClient.As<BahamutCacheModel>();
                    var key = BahamutCacheModel.GenerateCacheModelListKey(appUniqueId, modelType, uniqueId);
                    return client.Lists[key].Where(m => m.Type == modelType && m.UniqueId == uniqueId);
                }
            });
        }
    }

    
}
