using StackExchange.Redis;

namespace SampleMVC.Singletons
{
    internal class Redis
    {
        public static readonly Redis Instance = new Redis();
        private readonly string redisHost = "ip:port";

        private ConnectionMultiplexer connection;

        private Redis()
        {
        }

        public ConnectionMultiplexer Connection
        {
            get
            {
                return this.connection ?? (this.connection = ConnectionMultiplexer.Connect(this.redisHost));
            }
        }
    }
}