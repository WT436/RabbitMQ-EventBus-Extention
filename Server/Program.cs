using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Server
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

            using (var _rabbitMQConnection = connectionFactory.CreateConnection("ConnectionApp"))
            using (var channel = _rabbitMQConnection.CreateModel())
            {
                var consumer = InitializerConsumer(channel, "MyQueue");

                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var incommingMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine($"{DateTime.Now:o} Nhận được : {incommingMessage}");
                        var replyMessage = JsonSerializer.Serialize(incommingMessage);
                        if (Int32.TryParse(replyMessage.Replace("\"", ""), out int output))
                        {
                            output = fib(output);
                            Console.WriteLine($"Trả lời int : {output}");
                            SendReplyMessage(output.ToString(), channel, ea);
                        }
                        else
                        {
                            Console.WriteLine($"Trả lời string : {replyMessage}");
                            SendReplyMessage("Heloo : " + replyMessage, channel, ea);
                        }

                    }
                    catch
                    {
                    }
                };

                Console.ReadLine();
            }

            Console.WriteLine("End-Game!");
            Console.ReadKey();
        }

        private static void SendReplyMessage(string replyMessage, IModel channel, BasicDeliverEventArgs ea)
        {
            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            var responseBytes = Encoding.UTF8.GetBytes(replyMessage);
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                basicProperties: replyProps, body: responseBytes);
        }


        private static AsyncEventingBasicConsumer InitializerConsumer(IModel channel, string queueName)
        {
            channel.QueueDeclare(queue: queueName, durable: true,
                exclusive: false, autoDelete: false, arguments: null);

            channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            channel.BasicConsume(queue: queueName,
                autoAck: false, consumer: consumer);

            return consumer;
        }

        private static int fib(int n) => Enumerable.Range(1, n).Aggregate((t,i)=>t+i);
    }
}
