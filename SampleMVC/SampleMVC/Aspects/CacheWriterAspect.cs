using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using SampleMVC.Attributes;
using SampleMVC.Extensions;
using SampleMVC.Helpers;

namespace SampleMVC.Aspects
{
    public class CacheWriterAspect : IInterceptor
    {
        public int Order => 1;

        public void Intercept(IInvocation invocation)
        {
            var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheAttribute>();

            cacheAttribute = cacheAttribute
                             ?? new StackTrace().GetFrames()
                                 .Select(x => x.GetMethod().GetCustomAttribute<CacheAttribute>())
                                 .FirstOrDefault(x => x != null);

            if (IsWriteInCache(invocation, cacheAttribute))
            {
                var key = CacheKeyHelper.GenerateKey(
                    invocation.MethodInvocationTarget,
                    invocation.Arguments,
                    cacheAttribute.Template);

                SetCache(key, invocation.ReturnValue, cacheAttribute.Db, cacheAttribute.Timeout);
            }
        }

        private static bool IsWriteInCache(IInvocation invocation, CacheAttribute cacheAttribute)
        {
            return invocation.TryProceed() && invocation.Method.ReturnType != typeof(void) && cacheAttribute != null
                   && (cacheAttribute.Access & CacheAccess.Write) == CacheAccess.Write
                   && RedisHelper.Instance.Connection != null;
        }

        private static void SetCache(string key, object value, int db, TimeSpan? timeout)
        {
            try
            {
                var database = RedisHelper.Instance.Connection.GetDatabase(db);

                if (value != null)
                {
                    database.StringSet(key, JsonConvert.SerializeObject(value), timeout);
                }
                else
                {
                    database.KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                // TODO: Write Log
            }
        }
    }
}