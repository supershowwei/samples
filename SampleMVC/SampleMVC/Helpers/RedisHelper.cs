using System;
using StackExchange.Redis;

namespace SampleMVC.Helpers
{
    internal class RedisHelper : IDisposable
    {
        public static readonly RedisHelper Instance = new RedisHelper();
        private readonly string redisHost = "ip:port";

        private ConnectionMultiplexer connection;

        private RedisHelper()
        {
        }

        public ConnectionMultiplexer Connection
            =>
                this.connection == null || !this.connection.IsConnected
                    ? (this.connection = ConnectionMultiplexer.Connect(this.redisHost))
                    : this.connection;

        public void Dispose()
        {
            this.connection?.Dispose();
        }
    }
}