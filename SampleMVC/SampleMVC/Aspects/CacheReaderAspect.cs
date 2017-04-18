using System;
using System.Reflection;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using SampleMVC.Attributes;
using SampleMVC.Helpers;
using SampleMVC.Singletons;
using StackExchange.Redis;

namespace SampleMVC.Aspects
{
    public class CacheReaderAspect : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheAttribute>();

            if (invocation.Method.ReturnType != typeof(void) && cacheAttribute != null)
            {
                var key = CacheKeyHelper.GenerateKey(
                    invocation.MethodInvocationTarget,
                    invocation.Arguments,
                    cacheAttribute.Template);

                var cacheValue = GetCache(key, cacheAttribute.Db);

                if (cacheValue.HasValue)
                {
                    invocation.ReturnValue = JsonConvert.DeserializeObject(cacheValue, invocation.Method.ReturnType);
                }
                else
                {
                    invocation.Proceed();
                }
            }
            else
            {
                invocation.Proceed();
            }
        }

        private static RedisValue GetCache(string key, int db)
        {
            try
            {
                var database = Redis.Instance.Connection.GetDatabase(db);

                return database.StringGet(key);
            }
            catch (Exception ex)
            {
                // TODO: Write Log
                return default(RedisValue);
            }
        }
    }
}