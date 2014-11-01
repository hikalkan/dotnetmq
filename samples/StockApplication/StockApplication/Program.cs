using System;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;
using StockCommonLib;

namespace StockApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to query a stock status");
            Console.ReadLine();

            //Connect to DotNetMQ
            var mdsClient = new MDSClient("StockClient");
            mdsClient.MessageReceived += mdsClient_MessageReceived;
            mdsClient.Connect();

            //Create a stock request message
            var stockQueryMessage = new StockQueryMessage { StockCode = "S01" };
            
            //Create a MDS message
            var requestMessage = mdsClient.CreateMessage();
            requestMessage.DestinationApplicationName = "StockServer";
            requestMessage.TransmitRule = MessageTransmitRules.NonPersistent;
            requestMessage.MessageData = GeneralHelper.SerializeObject(stockQueryMessage);

            //Send message and get response
            var responseMessage = requestMessage.SendAndGetResponse();

            //Get stock query result message from response message
            var stockResult = (StockQueryResultMessage) GeneralHelper.DeserializeObject(responseMessage.MessageData);

            //Write stock query result
            Console.WriteLine("StockCode          = " + stockResult.StockCode);
            Console.WriteLine("ReservedStockCount = " + stockResult.ReservedStockCount);
            Console.WriteLine("TotalStockCount    = " + stockResult.TotalStockCount);

            //Acknowledge received message
            responseMessage.Acknowledge();

            Console.ReadLine();

            //Disconnect from DotNetMQ server.
            mdsClient.Disconnect();
        }

        static void mdsClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Simply acknowledge other received messages
            e.Message.Acknowledge();
        }
    }
}
