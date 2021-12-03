using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Client
{
    class Program
    {

        private static IConnection _rabbitMQConnection;
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
            _rabbitMQConnection = connectionFactory.CreateConnection("ConnectionApp");

            // create Exchange
            using (var exchange = _rabbitMQConnection.CreateModel())
            {
                var replayQueue = $"MyQueue_return";
                var correlationId = Guid.NewGuid().ToString();

                exchange.ExchangeDeclare(exchange: "Send",
                                         type: ExchangeType.Direct,
                                         durable: true,
                                         autoDelete: false);
                // Create Queue
                exchange.QueueDeclare(queue: replayQueue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false);

                exchange.QueueDeclare(queue: "MyQueue",
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false);

                var consumer = new AsyncEventingBasicConsumer(exchange);

                consumer.Received += async (model, ea) =>
                {
                    if (correlationId == ea.BasicProperties.CorrelationId)
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        if(Int32.TryParse(message.Replace("\"",""), out int output))
                        {
                            Console.WriteLine($"Received Number Data : {output}");
                        }
                        else
                        {
                            Console.WriteLine($"Received {message}");
                        }
                        
                        return;
                    }
                };

                exchange.BasicConsume(queue: replayQueue, autoAck: true, consumer: consumer);

                var pros = exchange.CreateBasicProperties();

                pros.CorrelationId = correlationId;
                pros.ReplyTo = replayQueue;

                while (true)
                {
                    var myMessage = Console.ReadLine();
                    pros.DeliveryMode = 2;
                    exchange.BasicPublish(exchange: "Send",
                                          routingKey: "QueueKey",
                                          basicProperties: pros,
                                          body: Encoding.UTF8.GetBytes(myMessage));
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.WriteLine("End-Game!");
            Console.ReadKey();
        }
        private static int fib(int n) => n == 0 || n == 1 ? n : fib(n - 1) + fib(n - 2);
    }
}
