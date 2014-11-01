using System;
using MDS;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var server = new MDSServer();
                server.Start();

                Console.WriteLine("DotNetMQ server has started.");
                Console.WriteLine("Press enter to stop...");
                Console.ReadLine();

                server.Stop(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
