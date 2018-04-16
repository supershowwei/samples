using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using log4net;
using SampleMVC.Attributes;
using SampleMVC.Containers;
using SampleMVC.Helpers;

namespace SampleMVC.Aspects
{
    public class CacheRemoveAspect : IInterceptor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CacheReaderAspect));

        public int Order => 0;

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var uncacheAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<UncacheAttribute>();

            uncacheAttribute = uncacheAttribute ?? new StackTrace().GetFrames()
                                   .Select(x => x.GetMethod().GetCustomAttribute<UncacheAttribute>())
                                   .FirstOrDefault(x => x != null);

            var uncacheKeys = UncacheKeyContainer.Instance.Keys;

            if (uncacheAttribute != null && uncacheKeys.ContainsKey(invocation.MethodInvocationTarget))
            {
                RemoveKeys(invocation.MethodInvocationTarget, invocation.Arguments);
            }
        }

        private static void RemoveKeys(MethodInfo targetMethod, object[] targetArguments)
        {
            foreach (var method in UncacheKeyContainer.Instance.Keys[targetMethod])
            {
                var parameters = targetMethod.GetParameters();

                var arguments = method.GetParameters()
                    .Select(
                        p =>
                            {
                                var arg = parameters.SingleOrDefault(x => x.Name.Equals(p.Name));

                                return arg == null ? null : targetArguments[arg.Position];
                            })
                    .ToArray();

                var key = CacheKeyHelper.GenerateKey(method, arguments, string.Empty);

                DeleteCache(key, 0);
            }
        }

        private static void DeleteCache(string key, int db)
        {
            try
            {
                RedisHelper.Instance.Connection.GetDatabase(db).KeyDelete(key);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat(
                    "無法刪除 Cache。('{0}')\r\n\r\nStackTrace:\r\n{1}",
                    ex.GetBaseException().Message,
                    ex.StackTrace);
            }
        }

    }
}