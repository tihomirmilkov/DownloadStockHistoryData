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
    class Program
    {
        static void Main(string[] args)
        {
            var quotes = GetAllQuotes();

            //if (args.Count() > 0)
            //{
            //    switch (args[0])
            //    {
            //        // download all missing data - update stock history
            //        case "d":
            //            {
            //                var quotes = GetAllQuotes();

            //            }
            //            break;
            //    }
            //}
        }

        
    }
}
