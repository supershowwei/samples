using System;

namespace SampleMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DoNotUseCacheReaderAttribute : Attribute
    {
    }
}