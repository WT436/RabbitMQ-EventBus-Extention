using RabbitMQ_Extention;
using System;
using System.Threading;

namespace Server_One
{
    class Program
    {
        static void Main(string[] args)
        {
            var sv = new HopeLetter();
            sv.ServerCallMess(queueName: "MyQueue");
            Console.WriteLine("Hello World!");
        }
    }
}
