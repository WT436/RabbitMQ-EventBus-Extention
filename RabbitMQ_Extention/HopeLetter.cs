using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Extention.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RabbitMQ_Extention
{
    public class HopeLetter
    {
        private readonly ConnectionFactory connectionFactory;
        private static IConnection _rabbitMQConnection;
        private const int PERFETCH_SIZE_CONFIG = 0;
        private const int PERFETCH_COUNT_CONFIG = 1;
        private const bool GLOBAL_CONFIG = false;

        public HopeLetter(RabbitMqConfiguration rabbitMqConfiguration)
        {
            try
            {
                string rabbitmqconnection = $"amqp://{HttpUtility.UrlEncode(rabbitMqConfiguration.UserName)}:{HttpUtility.UrlEncode(rabbitMqConfiguration.Password)}@{rabbitMqConfiguration.HostName}:{rabbitMqConfiguration.Port}/{HttpUtility.UrlEncode(rabbitMqConfiguration.VirtualHost)}";

                connectionFactory = new ConnectionFactory();
                connectionFactory.Uri = new Uri(rabbitmqconnection);
                connectionFactory.AutomaticRecoveryEnabled = rabbitMqConfiguration.AutomaticRecoveryEnabled;
                connectionFactory.DispatchConsumersAsync = rabbitMqConfiguration.DispatchConsumersAsync;
                connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(10);
                _rabbitMQConnection = connectionFactory.CreateConnection(rabbitMqConfiguration.ConnectionFactoryName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ServerCallMess(string queueName)
        {
            using var channel = _rabbitMQConnection.CreateModel();
            while (true)
            {
                var consumer = InitializerConsumer(channel, queueName);

                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var incommingMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"{DateTime.Now:hh-mm-ss dd/mm/yyyy} Nhan duoc : {incommingMessage}");
                        var replyMessage = JsonSerializer.Serialize(incommingMessage);
                        SendReplyMessage(replyMessage.ToString(), channel, ea);
                    }
                    catch
                    {
                    }
                };
            }
        }

        private static void SendReplyMessage(string replyMessage, IModel channel, BasicDeliverEventArgs ea)
        {
            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            var responseBytes = Encoding.UTF8.GetBytes(replyMessage);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
        }

        private static AsyncEventingBasicConsumer InitializerConsumer(IModel channel, string queueName)
        {
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.BasicQos(PERFETCH_SIZE_CONFIG, PERFETCH_COUNT_CONFIG, GLOBAL_CONFIG);

            var consumer = new AsyncEventingBasicConsumer(channel);

            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            return consumer;
        }

        public string ClientCallMess(string myMessage, string queueName)
        {
            string messRep = String.Empty;
            // create Exchange
            using (var exchange = _rabbitMQConnection.CreateModel())
            {
                var replayQueue = $"{queueName}_rep";
                var correlationId = Guid.NewGuid().ToString();

                // Create Queue
                exchange.QueueDeclare(queue: replayQueue, durable: true, exclusive: false, autoDelete: false);

                exchange.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

                var consumer = new AsyncEventingBasicConsumer(exchange);

                consumer.Received += async (model, ea) =>
                {
                    if (correlationId == ea.BasicProperties.CorrelationId)
                    {
                        var body = ea.Body.ToArray();
                        messRep = Encoding.UTF8.GetString(body);
                        return;
                    }
                };

                exchange.BasicConsume(queue: replayQueue, autoAck: true, consumer: consumer);

                var pros = exchange.CreateBasicProperties();

                pros.CorrelationId = correlationId;
                pros.ReplyTo = replayQueue;

                exchange.BasicPublish(exchange: "", routingKey: queueName, basicProperties: pros, body: Encoding.UTF8.GetBytes(myMessage));

                while (messRep == String.Empty)
                {
                    Thread.Sleep(10);
                }
            }

            return messRep;
        }
    }
}
