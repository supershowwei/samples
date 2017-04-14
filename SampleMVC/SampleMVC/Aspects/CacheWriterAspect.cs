﻿using System;
using System.Reflection;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using SampleMVC.Attributes;
using SampleMVC.Helpers;
using SampleMVC.Singletons;

namespace SampleMVC.Aspects
{
    public class CacheWriterAspect : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var cacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheAttribute>();

            if (invocation.Method.ReturnType != typeof(void) && cacheAttribute != null)
            {
                var key = CacheKeyHelper.GenerateKey(
                    invocation.MethodInvocationTarget,
                    invocation.Method.GetParameters(),
                    invocation.Arguments,
                    cacheAttribute.Template);

                SetCache(key, invocation.ReturnValue, cacheAttribute.Db, cacheAttribute.Timeout);
            }
        }

        private static void SetCache(string key, object value, int db, TimeSpan? timeout)
        {
            try
            {
                var database = Redis.Instance.Connection.GetDatabase(db);

                database.StringSet(key, JsonConvert.SerializeObject(value), timeout);
            }
            catch (Exception ex)
            {
                // TODO: Write Log
            }
        }
    }
}