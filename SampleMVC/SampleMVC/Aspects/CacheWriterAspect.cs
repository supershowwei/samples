using System;
using System.Reflection;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using SampleMVC.Attributes;
using SampleMVC.Helpers;

namespace SampleMVC.Aspects
{
    public class CacheWriterAspect : IInterceptor
    {
        public int Order => 1;

        public void Intercept(IInvocation invocation)
        {
            var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheAttribute>();

            if (TryProceed(invocation, out object returnValue) && invocation.Method.ReturnType != typeof(void)
                && cacheAttribute != null)
            {
                var key = CacheKeyHelper.GenerateKey(
                    invocation.MethodInvocationTarget,
                    invocation.Arguments,
                    cacheAttribute.Template);

                SetCache(key, returnValue, cacheAttribute.Db, cacheAttribute.Timeout);
            }
        }

        private static bool TryProceed(IInvocation invocation, out object returnValue)
        {
            returnValue = null;

            try
            {
                invocation.Proceed();

                returnValue = invocation.ReturnValue;

                return true;
            }
            catch (InvalidOperationException)
            {
                // 序列未包含符合的項目，代表已被刪除。
                return true;
            }
            catch (Exception ex)
            {
                // TODO: Write Log
                return false;
            }
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