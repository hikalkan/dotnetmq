using System;

namespace StockCommonLib
{
    [Serializable]
    public class StockQueryResultMessage
    {
        public string StockCode { get; set; }
        public int ReservedStockCount { get; set; }
        public int TotalStockCount { get; set; }
    }
}
