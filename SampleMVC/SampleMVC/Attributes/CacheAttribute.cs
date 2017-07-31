using System;

namespace SampleMVC.Attributes
{
    [Flags]
    public enum CacheAccess
    {
        Read = 1,
        Write = 2,
        ReadWrite = 3
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CacheAttribute" /> class.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <param name="template">The template. default: ClassName.Method({key}), you can customize it, ex: MyKey({key}).</param>
        /// <param name="db">The database.</param>
        /// <param name="timeout">The timeout. (second unit)</param>
        public CacheAttribute(
            CacheAccess access = CacheAccess.ReadWrite,
            string template = null,
            int db = 0,
            int timeout = 0)
        {
            this.Access = access;
            this.Template = template;
            this.Db = db;
            this.Timeout = timeout > 0 ? (TimeSpan?)TimeSpan.FromSeconds(timeout) : null;
        }

        public CacheAccess Access { get; private set; }

        public string Template { get; private set; }

        public int Db { get; private set; }

        public TimeSpan? Timeout { get; private set; }
    }
}