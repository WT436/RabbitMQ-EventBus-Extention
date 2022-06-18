using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Extention.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Extention
{
    public class RabbitMQService : IRabbitMQ
    {
        private readonly RabbitMqConfiguration _rabbitMqConfiguration;

        public RabbitMQService(RabbitMqConfiguration rabbitMqConfiguration)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration;
            InitRabbitMQService();
        }

        private IConnection _connection;
        private static IModel _channel;

        public void InitRabbitMQService()
        {
            // create factory
            var connectionFactory = new ConnectionFactory
            {
                HostName = _rabbitMqConfiguration.HostName,
                UserName = _rabbitMqConfiguration.UserName,
                Password = _rabbitMqConfiguration.Password
            };

            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.DispatchConsumersAsync = true;

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void SendMessager(string message, string exchangeName, string queueName, string routingKey)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
            // Create Queue
            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            // Bind Queue with Exchange
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
            // Publish Message
            var pro = _channel.CreateBasicProperties();
            pro.DeliveryMode = 2;
            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: pro, body: Encoding.UTF8.GetBytes(message));
        }

        public string ReceiveMessager()
        {
            // Subscribe for the Message
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            while (true)
            {
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += ConsumerReceived;
                _channel.BasicConsume(queue: "MyQueue", autoAck: false, consumer: consumer);
            }
        }

        private static async Task ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var mess = Encoding.UTF8.GetString(e.Body.ToArray());
            _channel.BasicAck(e.DeliveryTag, false);
            Console.WriteLine(mess);
        }

    }
}
