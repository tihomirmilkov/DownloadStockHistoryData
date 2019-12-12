using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceAPI.Models;

namespace DownloadStockHistoryData
{
    class QuoteSymbols
    {
        private List<string> allSymbolChars = new List<string>()
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                "^", "-", ".", ":", "\\",
            };

        private int allSymbolCharsCount;

        public QuoteSymbols()
        {
            allSymbolCharsCount = allSymbolChars.Count;
        }

        public void LoadAllQuotes(int start = 0, string symbol = "")
        {
            for (int i = start; i < allSymbolCharsCount; i++)
            {
                symbol += allSymbolChars[i];
                var quotes = CheckAutoCompleteForQuotes(symbol);

                // TODO - BUG fuck "A" returns only 9 results. Must figure out how to check symbols another way.
                // TODO Check if symbols are < 10
                // TODO Save in DB
                // TODO Publish and connect to DB successfully
            }
        }

        private List<string> CheckAutoCompleteForQuotes(string checkStr)
        {
            List<string> result = new List<string>();

            var client = new RestClient("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/auto-complete?lang=en&region=US&query=" + checkStr);
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "apidojo-yahoo-finance-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "24af7fd5b2msh3c45c5735b57c13p1dc477jsn00e0e44c4cd5");
            IRestResponse response = client.Execute(request);

            var responseQuotes = JsonConvert.DeserializeObject<ResultAutoCompleteQuoteData>(response.Content);

            result = responseQuotes.ResultSet.Result.Select(x => x.Symbol).ToList();

            return result;
        }
    }
}
