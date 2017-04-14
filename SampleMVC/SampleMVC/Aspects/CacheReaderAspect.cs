﻿using System.Reflection;
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
            var cacheKeyAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheKeyAttribute>();

            if (invocation.Method.ReturnType != typeof(void) && cacheKeyAttribute != null)
            {
                var key = CacheKeyHelper.GenerateKey(
                    invocation.MethodInvocationTarget,
                    invocation.Method.GetParameters(),
                    invocation.Arguments,
                    cacheKeyAttribute.Template);

                var db = Redis.Instance.Connection.GetDatabase(cacheKeyAttribute.Db);

                RedisValue cacheValue;
                if ((cacheValue = db.StringGet(key)).HasValue)
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
    }
}