using System;
using System.ServiceModel;

namespace ConsoleServerHost
{
    class Program
    {
        static void Main()
        {
            using(var host = new ServiceHost(typeof(ServerWork.ServerService)))
            {
                host.Open();

                Console.WriteLine("Host started...");
                Console.ReadLine();
            }
        }
    }
}
