using System;
using MDS;
using MDS.Client;
using StockCommonLib;

namespace StockServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var mdsClient = new MDSClient("StockServer");
            mdsClient.MessageReceived += MDSClient_MessageReceived;

            mdsClient.Connect();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

            mdsClient.Disconnect();
        }

        static void MDSClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Get message
            var stockQueryMessage = GeneralHelper.DeserializeObject(e.Message.MessageData) as StockQueryMessage;
            if (stockQueryMessage == null)
            {
                return;
            }

            //Write message content
            Console.WriteLine("Stock Query Message for: " + stockQueryMessage.StockCode);

            //Get stock counts from a database...
            int reservedStockCount;
            int totalStockCount;
            switch (stockQueryMessage.StockCode)
            {
                case "S01":
                    reservedStockCount = 14;
                    totalStockCount = 80;
                    break;
                case "S02":
                    reservedStockCount = 0;
                    totalStockCount = 25;
                    break;
                default:
                    reservedStockCount = -1;
                    totalStockCount = -1;
                    break;
            }

            //Create a reply message for stock query
            var stockQueryResult = new StockQueryResultMessage
                                       {
                                           StockCode = stockQueryMessage.StockCode,
                                           ReservedStockCount = reservedStockCount,
                                           TotalStockCount = totalStockCount
                                       };
            
            //Create a MDS response message to send to client
            var responseMessage = e.Message.CreateResponseMessage();
            responseMessage.MessageData = GeneralHelper.SerializeObject(stockQueryResult);

            //Send message
            responseMessage.Send();

            //Acknowledge the original request message. So, it will be deleted from queue.
            e.Message.Acknowledge();
        }
    }
}
