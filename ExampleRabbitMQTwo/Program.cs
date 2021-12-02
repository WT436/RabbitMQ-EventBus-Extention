using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ExampleRabbitMQTwo
{
    static class Program
    {
        private static IModel _exchange;

        static void Main(string[] args)
        {
            // thu gọn kết nối
            string rabbitmqconnection = $"amqp://{HttpUtility.UrlEncode("WT436Admin")}"
                                      + $":{ HttpUtility.UrlEncode("WT436Admin")}"
                                      + $"@{ "localhost"}"
                                      + $":{"5672"}"
                                      + $"/{ HttpUtility.UrlEncode("TranHaiNam")}";
            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri(rabbitmqconnection);
            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.DispatchConsumersAsync = true;

          var _rabbitMQConnection = connectionFactory.CreateConnection("ConnectionApp");

            // create Exchange
            _exchange =  _rabbitMQConnection.CreateModel();

            // Subscribe for the Message
            _exchange.BasicQos(prefetchSize: 0,
                              prefetchCount: 1,
                              global: false);

            var consumer = new AsyncEventingBasicConsumer(_exchange);
            consumer.Received += ConsumerReceived;
            _exchange.BasicConsume(queue: "MyQueue", autoAck: false, consumer: consumer);

            Console.WriteLine("End-Game!");
            Console.ReadKey();
        }

        private static async Task ConsumerReceived(object sender, BasicDeliverEventArgs e)
        {
            var mess = Encoding.UTF8.GetString(e.Body.ToArray());
            _exchange.BasicAck(e.DeliveryTag,false);
            Console.WriteLine(mess);
        }
    }
}
