using System;

namespace SampleMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CacheKeyAttribute : Attribute
    {
        public CacheKeyAttribute(string template, int db = 0, TimeSpan? timeout = null)
        {
            this.Template = template;
            this.Db = db;
            this.Timeout = timeout;
        }

        public int Db { get; set; }

        public string Template { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}