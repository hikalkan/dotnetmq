using System;

namespace StockCommonLib
{
    [Serializable]
    public class StockQueryMessage
    {
        public string StockCode { get; set; }
    }
}
