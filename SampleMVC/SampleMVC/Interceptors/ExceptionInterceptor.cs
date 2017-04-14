using System;
using Castle.DynamicProxy;
using log4net;
using Newtonsoft.Json;
using SampleMVC.Models;

namespace SampleMVC.Interceptors
{
    public class ExceptionInterceptor : IInterceptor
    {
        private ILog log;

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                this.log = LogManager.GetLogger(invocation.InvocationTarget.GetType());

                var serviceResult = Activator.CreateInstance(invocation.Method.ReturnType) as ServiceResult;
                serviceResult.IsFailure = true;
                serviceResult.Message = ex.GetBaseException().Message;

                this.log.ErrorFormat(
                    "Call '{0}.{1}' occur error '{2}'. Arguments: {3}",
                    invocation.InvocationTarget.GetType().Name,
                    invocation.Method.Name,
                    serviceResult.Message,
                    SerializeArguments(invocation.Arguments));

                invocation.ReturnValue = serviceResult;
            }
        }

        private static string SerializeArguments(object[] arguments)
        {
            string serialized;

            try
            {
                serialized = JsonConvert.SerializeObject(arguments);
            }
            catch (Exception ex)
            {
                serialized = string.Concat("參數無法序列化成 JSON", ex.GetBaseException().Message);
            }

            return serialized;
        }
    }
}