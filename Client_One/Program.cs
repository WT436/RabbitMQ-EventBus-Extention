using RabbitMQ_Extention;
using RabbitMQ_Extention.Collections;
using System;
using System.Threading.Tasks;

namespace Client_One
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
            int i = 0;
            //while(true)
            //{
            //    i++;

                string input = "Mess gui di : " + i.ToString();
                Console.WriteLine(input);
                var data = sv.ClientCallMess(input, queueName: "MyQueue");
                Console.WriteLine($"mess nhan ve {i} : " + data);
            //}
        }
    }
}
