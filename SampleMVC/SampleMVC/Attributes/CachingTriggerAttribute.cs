using System;

namespace SampleMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CachingTriggerAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingTriggerAttribute" /> class.
        /// </summary>
        /// <param name="operation">I: Insert, U: Update, D: Delete</param>
        /// <param name="channels">The channels.</param>
        public CachingTriggerAttribute(string operation, params string[] channels)
        {
            this.Operation = operation;
            this.Channels = channels;
        }

        public string Operation { get; set; }

        public string[] Channels { get; set; }
    }
}