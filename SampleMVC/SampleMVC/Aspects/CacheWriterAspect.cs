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

            var cacheKeyAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheKeyAttribute>();

            if (invocation.Method.ReturnType != typeof(void) && cacheKeyAttribute != null)
            {
                var key = CacheKeyHelper.GenerateKey(
                    invocation.MethodInvocationTarget,
                    invocation.Method.GetParameters(),
                    invocation.Arguments,
                    cacheKeyAttribute.Template);

                var db = Redis.Instance.Connection.GetDatabase(cacheKeyAttribute.Db);

                db.StringSet(key, JsonConvert.SerializeObject(invocation.ReturnValue), cacheKeyAttribute.Timeout);
            }
        }
    }
}