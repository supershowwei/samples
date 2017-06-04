using System;

namespace SampleMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CacheAttribute" /> class.
        /// </summary>
        /// <param name="template">The template. default: ClassName.Method({key}), you can customize it, ex: MyKey({key}).</param>
        /// <param name="db">The database.</param>
        /// <param name="timeout">The timeout. (second unit)</param>
        /// <param name="excludedCallers">The excluded callers.</param>
        public CacheAttribute(string template, int db = 0, TimeSpan? timeout = null, string[] excludedCallers = null)
        {
            this.Template = template;
            this.Db = db;
            this.Timeout = timeout;
            this.ExcludedCallers = excludedCallers;
        }

        public string Template { get; set; }

        public int Db { get; set; }

        public TimeSpan? Timeout { get; set; }

        public string[] ExcludedCallers { get; set; }
    }
}