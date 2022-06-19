using RabbitMQ_Extention;
using System;
using System.Threading.Tasks;

namespace Client_One
{
    class Program
    {
        static void Main(string[] args)
        {
            var sv = new HopeLetter();
            int i = 0;
            while(true)
            {
                i++;

                string input = "Mess gui di : " + i.ToString();
                Console.WriteLine(input);
                var data = sv.ClientCallMess(input, queueName: "MyQueue");
                Console.WriteLine($"mess nhan ve {i} : " + data);
            }
        }
    }
}
