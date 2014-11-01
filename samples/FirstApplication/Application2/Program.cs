using System;
using System.Text;
using MDS.Client;

namespace Application2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create MDSClient object to connect to DotNetMQ
            //Name of this application: Application2
            var mdsClient = new MDSClient("Application2");

            //Register to MessageReceived event to get messages.
            mdsClient.MessageReceived += MDSClient_MessageReceived;

            //Connect to DotNetMQ server
            mdsClient.Connect();

            //Wait user to press enter to terminate application
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

            //Disconnect from DotNetMQ server
            mdsClient.Disconnect();
        }

        /// <summary>
        /// This method handles received messages from other applications via DotNetMQ.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Message parameters</param>
        static void MDSClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Get message
            var messageText = Encoding.UTF8.GetString(e.Message.MessageData);

            //Process message
            Console.WriteLine();
            Console.WriteLine("Text message received : " + messageText);
            Console.WriteLine("Source application    : " + e.Message.SourceApplicationName);

            //Acknowledge that message is properly handled and processed. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }
    }
}
