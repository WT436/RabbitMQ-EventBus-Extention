using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ_Extention.Collections
{
    public sealed class RabbitMqConfiguration
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string HostName { get; set; }
        public string Port { get; set; }

        public string QueueName { get; set; }
        public string ExchangeName { get; set; }
    }
}
