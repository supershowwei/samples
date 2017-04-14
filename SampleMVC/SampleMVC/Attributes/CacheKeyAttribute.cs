using System;

namespace SampleMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CacheKeyAttribute : Attribute
    {
        public CacheKeyAttribute(string template, int db = 0)
        {
            this.Template = template;
            this.Db = db;
        }

        public int Db { get; set; }

        public string Template { get; set; }
    }
}