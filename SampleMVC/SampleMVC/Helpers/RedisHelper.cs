using System;
using StackExchange.Redis;

namespace SampleMVC.Helpers
{
    internal class RedisHelper : IDisposable
    {
        public static readonly RedisHelper Instance = new RedisHelper();
        private readonly string redisHost = "{Host}:{Port}";

        private ConnectionMultiplexer connection;

        private RedisHelper()
        {
        }

        public ConnectionMultiplexer Connection
        {
            get
            {
                if (string.IsNullOrEmpty(this.redisHost)) return null;

                return this.connection == null || !this.connection.IsConnected
                    ? (this.connection = this.TryConnect())
                    : this.connection;
            }
        }

        public void Dispose()
        {
            this.connection?.Dispose();
        }

        private ConnectionMultiplexer TryConnect()
        {
            try
            {
                return ConnectionMultiplexer.Connect(this.redisHost);
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}