using System;
using System.Text;
using MDS.Client;

namespace Application1
{
    class Program
    {
        static void Main()
        {
            //Create MDSClient object to connect to DotNetMQ
            //Name of this application: Application1
            var mdsClient = new MDSClient("Application1");

            //Connect to DotNetMQ server
            mdsClient.Connect();

            Console.WriteLine("Write a text and press enter to send to Application2. Write 'exit' to stop application.");

            while (true)
            {
                //Get a message from user
                var messageText = Console.ReadLine();
                if (string.IsNullOrEmpty(messageText) || messageText == "exit")
                {
                    break;
                }

                //Create a DotNetMQ Message to send to Application2
                var message = mdsClient.CreateMessage();
                //Set destination application name
                message.DestinationApplicationName = "Application2";
                //message.DestinationServerName = "this_server2";
                //Set message data
                message.MessageData = Encoding.UTF8.GetBytes(messageText);

                //Send message
                message.Send();
            }

            //Disconnect from DotNetMQ server
            mdsClient.Disconnect();
        }
    }
}
