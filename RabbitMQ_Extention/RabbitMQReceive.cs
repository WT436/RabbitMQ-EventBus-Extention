using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RabbitMQ_Extention
{
    public class RabbitMQReceive
    {
        private static IModel _exchange;
        public RabbitMQReceive()
        {
            string rabbitmqconnection = $"amqp://{HttpUtility.UrlEncode("WT436")}"
                                      + $":{ HttpUtility.UrlEncode("WT436")}"
                                      + $"@{ "localhost"}"
                                      + $":{"5672"}"
                                      + $"/{ HttpUtility.UrlEncode("VHTest")}";
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri(rabbitmqconnection);
            connectionFactory.AutomaticRecoveryEnabled = true;

            connectionFactory.DispatchConsumersAsync = true;
            var _rabbitMQConnection = connectionFactory.CreateConnection("ConnectionApp");

            // create Exchange
            _exchange = _rabbitMQConnection.CreateModel();

            // Subscribe for the Message
            _exchange.BasicQos(prefetchSize: 0,
                              prefetchCount: 1,
                              global: false);

            var consumer = new AsyncEventingBasicConsumer(_exchange);
            consumer.Received += ConsumerReceived;
            _exchange.BasicConsume(queue: "MyQueue", autoAck: false, consumer: consumer);
        }

        private static async Task ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var mess = Encoding.UTF8.GetString(e.Body.ToArray());
            _exchange.BasicAck(e.DeliveryTag, false);
        }

    }
}
