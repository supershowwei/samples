using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using log4net;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SampleMVC.Attributes;
using SampleMVC.Helpers;

namespace SampleMVC.Aspects
{
    public class CacheWritingTriggerAspect : IInterceptor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CacheWritingTriggerAspect));

        public int Order => -1;

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            var cachingTriggerAttribute =
                invocation.MethodInvocationTarget.GetCustomAttribute<CachingTriggerAttribute>();

            if (cachingTriggerAttribute != null)
            {
                var message = GenerateMessage(invocation.Method.GetParameters(), invocation.Arguments);

                foreach (var channel in cachingTriggerAttribute.Channels)
                {
                    TriggerCaching(channel, message);
                }
            }
        }

        private static Dictionary<string, object> GenerateMessage(ParameterInfo[] parameters, object[] arguments)
        {
            var message = parameters.Zip(arguments, (p, o) => new { p.Name, Value = o })
                .ToDictionary(x => x.Name, x => x.Value);

            return message;
        }

        private static void TriggerCaching(string channel, Dictionary<string, object> message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                RabbitMQHelper.Instance.GetChannel(channel).BasicPublish(string.Empty, channel, body: body);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("無法發送 Caching Trigger。('{0}')", ex.GetBaseException().Message);
            }
        }
    }
}