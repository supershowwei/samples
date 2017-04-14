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

                var db = Redis.Instance.Connection.GetDatabase(cacheAttribute.Db);

                db.StringSet(key, JsonConvert.SerializeObject(invocation.ReturnValue), cacheAttribute.Timeout);
            }
        }
    }
}