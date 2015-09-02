using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BahamutCommon
{
    public interface IBahamutServiceBuilder
    {
        IBahamutServiceProvider ServiceProvider { get; }
        IDictionary<string, object> Properties { get; }
    }

    public interface IBahamutServiceProvider
    {
        object GetService(Type type);
        object GetService(Type type, params object[] args);
    }

    public static class GetBahamutServiceExtensions
    {
        public static T GetService<T>(this IBahamutServiceBuilder builder)
        {
            var typeName = typeof(T).FullName;
            if (builder.Properties.ContainsKey(typeName))
            {
                return (T)builder.Properties[typeName];
            }
            else
            {
                var ex = new Exception(string.Format("Never Use {0} Before!", typeName));
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }

    public static class UseBahamutServiceExtensions
    {
        public static IBahamutServiceBuilder UseService(this IBahamutServiceBuilder builder, Type BahamutService, params object[] args)
        {
            object service;
            if (args == null || args.Length == 0)
            {
                service = builder.ServiceProvider.GetService(BahamutService);
            }
            else
            {
                service = builder.ServiceProvider.GetService(BahamutService, args);
            }
            var typeName = BahamutService.FullName;
            builder.Properties.Add(typeName, service);
            Console.WriteLine("Use Service:" + typeName);
            return builder;
        }

        public static IBahamutServiceBuilder UseService<T>(this IBahamutServiceBuilder builder, params object[] args)
        {
            return UseService(builder, typeof(T), args);
        }
    }
}
