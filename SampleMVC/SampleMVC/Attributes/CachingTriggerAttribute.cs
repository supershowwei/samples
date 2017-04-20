using System;

namespace SampleMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CachingTriggerAttribute : Attribute
    {
        public CachingTriggerAttribute(params string[] channels)
        {
            this.Channels = channels;
        }

        public string[] Channels { get; set; }
    }
}