using System;
using SampleMVC.Models;

namespace SampleMVC.Extensions
{
    public static class ServiceResultExtension
    {
        public static TResult To<TSource, TResult>(this TSource me, Func<TSource, TResult> selector)
            where TSource : ServiceResult
        {
            return selector(me);
        }
    }
}