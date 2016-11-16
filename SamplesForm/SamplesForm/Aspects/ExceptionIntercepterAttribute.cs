namespace SamplesForm.Aspects
{
    [MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Public,
         Inheritance = MulticastInheritance.Multicast)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    [PSerializable]
    public class ExceptionIntercepterAttribute : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            var methodInfo = args.Method as MethodInfo;

            var log = LogManager.GetLogger(methodInfo.DeclaringType);

            var serializedArguments = SerializeArguments(methodInfo.GetParameters(), args.Arguments);

            var serviceResult = Activator.CreateInstance(methodInfo.ReturnType) as ServiceResult;
            serviceResult.Message = args.Exception.GetBaseException().Message;
            serviceResult.IsFailure = true;

            log.ErrorFormat(
                "發生無法預期的錯誤：{0}\r\n方法：{1}\r\n參數：{2}\r\nStackTrace:\r\n{3}",
                serviceResult.Message,
                string.Format("{0}.{1}", methodInfo.ReflectedType.Name, methodInfo.Name),
                serializedArguments,
                args.Exception.StackTrace);

            args.ReturnValue = serviceResult;
            args.FlowBehavior = FlowBehavior.Return;
        }

        private static string SerializeArgument(object argument)
        {
            try
            {
                return JsonConvert.SerializeObject(argument);
            }
            catch
            {
                return "Cannot serialized.";
            }
        }

        private static string SerializeArguments(ParameterInfo[] parameters, Arguments arguments)
        {
            var dic = parameters.Select((p, i) => new { Key = p.Name, Value = arguments[i] })
                .ToDictionary(x => x.Key, x => SerializeArgument(x.Value));

            return JsonConvert.SerializeObject(dic);
        }
    }
}