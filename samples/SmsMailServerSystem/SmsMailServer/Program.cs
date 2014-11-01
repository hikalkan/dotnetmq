using System;
using MDS.Client.MDSServices;

namespace SmsMailServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = new MDSServiceApplication("MyMailSmsService"))
            {
                service.AddService(new MyMailSmsService());
                service.Connect();

                Console.WriteLine("Press any key to stop service");
                Console.ReadLine();
            }
        }
    }
}
