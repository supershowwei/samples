using System;
using SampleMVC.Protocol.Model.Results;

namespace SampleMVC.Protocol.Extensions
{
    public static class ServiceResultExtension
    {
        public static TResult Readable<TValue, TResult>(
            this ServiceResult<TValue> me,
            Func<ServiceResult<TValue>, TResult> selector)
        {
            return selector(me);
        }

        public static TResult Readable<TValue1, TValue2, TResult>(
            this ServiceResult<TValue1, TValue2> me,
            Func<ServiceResult<TValue1, TValue2>, TResult> selector)
        {
            return selector(me);
        }
    }
}