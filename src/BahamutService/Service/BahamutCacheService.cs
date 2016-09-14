using BahamutCommon;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
            return GenerateCacheModelListKey(AppUniqueId, Type);
        }

        public static string GenerateCacheModelListKey(string appUniqueId, string modelType)
        {
            return string.Format("{0}:{1}", appUniqueId, modelType);
        }

        public static BahamutCacheModel FromJson(string json)
        {
            return JsonConvert.DeserializeObject<BahamutCacheModel>(json);
        }
    }

    public class BahamutCacheService
    {
        private ConnectionMultiplexer redis;
        public BahamutCacheService(ConnectionMultiplexer redis)
        {
            this.redis = redis;
        }

        public void PushCacheModelToList(BahamutCacheModel model)
        {
            Task.Run(() =>
            {
                var db = redis.GetDatabase();
                var key = model.GetCacheModelListKey();
                db.ListRightPush(key, model.ToJson());
            });
        }


        public void ClearCacheModels(string appUniqueId, string modelType)
        {
            Task.Run(() =>
            {
                var key = BahamutCacheModel.GenerateCacheModelListKey(appUniqueId, modelType);
                redis.GetDatabase().KeyDelete(key);
            });
        }

        public async Task<IEnumerable<BahamutCacheModel>> GetCacheModelsAsync(string appUniqueId, string modelType)
        {
            return await Task.Run(() =>
            {
                var key = BahamutCacheModel.GenerateCacheModelListKey(appUniqueId, modelType);
                var values = redis.GetDatabase().ListRange(key);
                var result = from v in values select BahamutCacheModel.FromJson(v);
                return result;
            });
        }
    }

    public static class GetBahamutCacheServiceExtension
    {
        public static BahamutCacheService GetBahamutCacheService(this IServiceProvider provider)
        {
            return provider.GetService<BahamutCacheService>();
        }
    }

}
