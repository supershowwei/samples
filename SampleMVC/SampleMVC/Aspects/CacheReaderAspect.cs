using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using SampleMVC.Attributes;
using SampleMVC.Extensions;
using SampleMVC.Helpers;
using StackExchange.Redis;

namespace SampleMVC.Aspects
{
    public class CacheReaderAspect : IInterceptor
    {
        public int Order => 0;

        public void Intercept(IInvocation invocation)
        {
            var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheAttribute>();

            if (IsReadFromCache(invocation, cacheAttribute))
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
                var database = RedisHelper.Instance.Connection.GetDatabase(db);

                return database.StringGet(key);
            }
            catch (Exception ex)
            {
                // TODO: Write Log
                return default(RedisValue);
            }
        }

        private static bool IsReadFromCache(IInvocation invocation, CacheAttribute cacheAttribute)
        {
            if (cacheAttribute == null) return false;
            if (invocation.Method.ReturnType == typeof(void)) return false;

            if (cacheAttribute.ExcludedCallers != null && cacheAttribute.ExcludedCallers.Length > 0)
            {
                return
                    !cacheAttribute.ExcludedCallers.Any(
                        m => new StackTrace().GetFrames().Where(x => x.GetMethod().ReflectedType != null).Select(
                            x =>
                                {
                                    var method = x.GetMethod();

                                    return string.Concat(method.ReflectedType.FullName, ".", method.Name);
                                }).Any(x => m.EqualsIgnoreCase(x)));
            }

            return true;
        }
    }
}