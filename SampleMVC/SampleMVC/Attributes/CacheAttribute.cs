using System;

namespace SampleMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CacheAttribute : Attribute
    {
        public CacheAttribute(string template, int db = 0, TimeSpan? timeout = null)
        {
            this.Template = template;
            this.Db = db;
            this.Timeout = timeout;
        }

        public string Template { get; set; }

        public int Db { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}