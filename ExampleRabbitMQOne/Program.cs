using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ExampleRabbitMQOne
{
    static class Program
    {
        private static IConnection _rabbitMQConnection;
        static void Main(string[] args)
        {
            string connectionString = "amqps://myhost?cacertfile=/path/to/ca_certificate.pem"
                                    + "& certfile =/path/to/client_certificate.pem" // chứng chỉ SSL
                                    + "& cacertfile =/path/to/client_key.pem" // chứng chỉ SSL
                                    + "& keyfile =/path/to/client_key.pem" // chứng chỉ SSL
                                    + "& verify = verify_peer" // Chỉ sử dụng cho lược đồ amqps và được sử dụng để định cấu hình xác minh chứng chỉ x509 (TLS) của máy chủ. Lưu ý: Chúng tôi rất khuyến khích sử dụng cả hai giá trị.
                                    + "& server_name_indication = myhost"// Chỉ sử dụng cho lược đồ amqps và được sử dụng để định cấu hình xác minh chứng chỉ x509 (TLS) của máy chủ. Lưu ý: Chúng tôi rất khuyến khích sử dụng cả hai giá trị.
                                    + "& auth_mechanism = myhost"// Các cơ chế xác thực SASL cần xem xét khi thương lượng một cơ chế với máy chủ.
                                    + "& heartbeat = myhost"// Giá trị thời gian chờ nhịp tim tính bằng giây (một số nguyên) để thương lượng với máy chủ.
                                    + "& connection_timeout = myhost"// Thời gian tính bằng mili giây (một số nguyên) để đợi trong khi thiết lập kết nối TCP với máy chủ trước khi từ bỏ.
                                    + "& channel_max = myhost"; //Số kênh tối đa cho phép trên kết nối này.

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

            while (true)
            {
                // create Exchange
                using (var exchange = _rabbitMQConnection.CreateModel())
                {
                    exchange.ExchangeDeclare(exchange: "Send",
                                             type: ExchangeType.Direct,
                                             durable: true,
                                             autoDelete: false);
                    // Create Queue
                    exchange.QueueDeclare(queue: "MyQueue",
                                          durable: true,
                                          exclusive: false,
                                          autoDelete: false);
                    // Bind Queue with Exchange
                    exchange.QueueBind(queue: "MyQueue",
                                       exchange: "Send",
                                       routingKey: "QueueKey");
                    // Publish Message
                    var myMessage = Console.ReadLine();
                    var pro = exchange.CreateBasicProperties();
                    pro.DeliveryMode = 2;
                    exchange.BasicPublish(exchange: "Send",
                                          routingKey: "QueueKey",
                                          basicProperties: pro,
                                          body: Encoding.UTF8.GetBytes(myMessage));
                }
            }
            Console.WriteLine("End-Game!");
            Console.ReadKey();
        }
    }
}
