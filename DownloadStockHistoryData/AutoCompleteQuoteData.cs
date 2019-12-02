using System;
using System.Collections.Generic;

namespace YahooFinanceAPI.Models
{
    public class AutoCompleteQuoteData
    {
        public string Symbol { get; set; }
        public string Exch { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ExchDisp { get; set; }
        public string TypeDisp { get; set; }
    }

    public class ResultSetAutoCompleteQuoteData
    {
        public string Query { get; set; }
        public List<AutoCompleteQuoteData> Result { get; set; }
    }

    public class ResultAutoCompleteQuoteData
    {
        public ResultSetAutoCompleteQuoteData ResultSet { get; set; }
    }
}