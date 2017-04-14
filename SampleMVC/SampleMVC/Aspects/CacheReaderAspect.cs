using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using SampleMVC.Attributes;
using SampleMVC.Singletons;
using StackExchange.Redis;

namespace SampleMVC.Aspects
{
    public class CacheReaderAspect : IInterceptor
    {
        private static readonly Regex NameRegex = new Regex(@"{(?<name>\w*)}");

        public void Intercept(IInvocation invocation)
        {
            var cacheKeyAttribute = invocation.MethodInvocationTarget.GetCustomAttribute<CacheKeyAttribute>();

            if (invocation.Method.ReturnType != typeof(void) && cacheKeyAttribute != null)
            {
                var key = GenerateKey(
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

        private static string GenerateKey(ParameterInfo[] parameters, object[] arguments, string input)
        {
            var argumentsWithName = GenerateArgumentsWithName(parameters, arguments);

            while (NameRegex.IsMatch(input))
            {
                input = NameRegex.Replace(
                    input,
                    m =>
                        argumentsWithName.ContainsKey(m.Groups["name"].Value)
                            ? argumentsWithName[m.Groups["name"].Value].ToString()
                            : string.Empty);
            }

            return input;
        }

        private static Dictionary<string, object> GenerateArgumentsWithName(ParameterInfo[] parameters, object[] args)
        {
            return parameters.Select((p, i) => new { p.Name, Argument = args[i] })
                .ToDictionary(x => x.Name, x => x.Argument);
        }
    }
}