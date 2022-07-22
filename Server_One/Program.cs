using RabbitMQ_Extention;
using RabbitMQ_Extention.Collections;
using System;
using System.Threading;

namespace Server_One
{
    class Program
    {
        static void Main(string[] args)
        {
            var sv = new HopeLetter(new RabbitMqConfiguration
            {
                UserName = "WT436",
                Password = "WT436",
                HostName = "localhost",
                Port = "5672",
                VirtualHost = "VHTest"
            });
            sv.ServerCallMess(queueName: "MyQueue");
            Console.WriteLine("Hello World!");
        }
    }
}
