using System;
using MDS.Client;
using MDS.Client.MDSServices;
using SampleService;

namespace SmsMailClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to test SendSms method");
            Console.ReadLine();

            //Application1 is name of an application that sends sms/email.
            using (var serviceConsumer = new MDSServiceConsumer("Application3"))
            {
                //Connect to DotNetMQ server
                serviceConsumer.Connect();

                //Create service proxy to call remote methods
                var service = new MyMailSmsServiceProxy(serviceConsumer, new MDSRemoteAppEndPoint("MyMailSmsService"));

                //Call SendSms method
                service.SendSms("3221234567", "Hello service!");
            }
        }
    }
}
