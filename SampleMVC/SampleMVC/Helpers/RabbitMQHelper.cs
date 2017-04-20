using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client;

namespace SampleMVC.Helpers
{
    internal class RabbitMQHelper : IDisposable
    {
        public static readonly RabbitMQHelper Instance = new RabbitMQHelper();

        private readonly ConnectionFactory connectionFactory = new ConnectionFactory
                                                                   {
                                                                       HostName = "host name",
                                                                       UserName = "user name",
                                                                       Password = "password"
                                                                   };

        private readonly Dictionary<string, IModel> channels = new Dictionary<string, IModel>();

        private IConnection connection;

        private RabbitMQHelper()
        {
        }

        public IConnection Connection
            =>
                this.connection == null || !this.connection.IsOpen
                    ? (this.connection = this.connectionFactory.CreateConnection())
                    : this.connection;

        public IModel GetChannel(string queue)
        {
            if (!this.channels.ContainsKey(queue) || !this.channels[queue].IsOpen)
            {
                var channel = this.Connection.CreateModel();

                channel.QueueDeclare(queue, true, false, false);

                this.channels[queue] = channel;
            }

            return this.channels[queue];
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted",
            Justification = "Reviewed. Suppression is OK here.")]
        public void Dispose()
        {
            this.connection?.Dispose();
        }
    }
}