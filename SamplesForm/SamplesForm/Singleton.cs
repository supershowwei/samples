using System.Runtime.Caching;

namespace SamplesForm
{
    public class Singleton
    {
        public static readonly Singleton Instance = new Singleton();
        private static readonly string Key = "MyKey";
        private readonly ObjectCache objectCache = MemoryCache.Default;

        private Singleton()
        {
        }

        public object CacheObject
        {
            get
            {
                if (this.objectCache[Key] == null)
                {
                    this.LoadCacheObject();
                }

                return this.objectCache[Key];
            }
        }

        private void LoadCacheObject()
        {
            var configPath = "Config File Path";

            var cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(new string[] { configPath }));

            this.objectCache.Set(Key, "object or value", cacheItemPolicy);
        }
    }
}