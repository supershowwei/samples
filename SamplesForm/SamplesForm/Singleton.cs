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
            throw new System.NotImplementedException();
        }
    }
}