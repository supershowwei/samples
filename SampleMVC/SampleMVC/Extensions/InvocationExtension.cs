using System;
using Castle.DynamicProxy;

namespace SampleMVC.Extensions
{
    public static class InvocationExtension
    {
        public static bool TryProceed(this IInvocation invocation)
        {
            try
            {
                invocation.Proceed();

                return true;
            }
            catch (InvalidOperationException)
            {
                // 序列未包含符合的項目，代表已被刪除。
                return true;
            }
        }
    }
}