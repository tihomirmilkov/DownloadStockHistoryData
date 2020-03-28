using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Windows.Forms;
using System.Threading;

namespace LevermannStrategyAutoEvaluator
{
    class TheMotherEvaluator
    {
        public TheMotherEvaluator()
        {
                
        }

        public LevermannParameters GetLevermannParameters(string stockQuote)
        {
            var result = new LevermannParameters();

            JObject detailsData = GetDetailsData(stockQuote);
            JObject financialsData = GetFinancialsData(stockQuote);
            string wsjHtmlCode = GetWsjHtmlCode(stockQuote);

            // 1. RoE - Return on Equity
            result.RoE = CalculateRoE(detailsData);

            // 2. EBIT margin
            result.EBITMargin = CalculateEBITMargin(financialsData);

            // 3. Equity ratio = total shareholders equity / total assets
            result.EquityRatio = CalculateEquityRatio(financialsData);

            // 4. P/E ratio 1 year
            result.PE1year = CalculatePE1year(detailsData);

            // 5. P/E ratio 5 years
            result.PE5years = CalculatePE5years(detailsData);

            // 6. Analyst opinions
            result.AnalystOpinions = CalculateAnalystOpinions(wsjHtmlCode);

            // 7. Reactions to quarterly figures relase
            result.ReactionToQuarterlyRelease = CalculateReactionToQuarterlyRelease(detailsData);

            // 8. Profit revision
            // The difference between the analysts’ estimates for earnings per share of 4 weeks ago is compared with the current expectations.
            // This is done for current fiscal year and next fiscal year.
            result.ProfitRevision = CalculateProfitRevision(wsjHtmlCode);

            // 9. Price change 6 months
            result.PriceChange6months = CalculatePriceChange6months(stockQuote);

            // 10. Price change 12 months
            result.PriceChange12months = CalculatePriceChange12months(stockQuote);

            // 11. Price momentum
            result.PriceChange12months = CalculatePriceMomentum(stockQuote);

            // 12. Reversal effect
            // Compare to the benchmark index for the last 3 months.
            result.ReversalEffect = CalculateReversalEffect(stockQuote);

            // 13. Profit growth
            // The difference between the profit forecast for the next year and the profit forecast for the current year.
            result.ProfitGrowth = CalculateProfitGrowth(wsjHtmlCode);

            return result;
        }

        private double CalculateProfitGrowth(string wsjHtmlCode)
        {
            string currentYear = DateTime.Now.Year.ToString();
            string nextYear = (DateTime.Now.Year + 1).ToString();

            string currYearBasic = FindSubstringWithBeginAndEnd(wsjHtmlCode, "FY " + currentYear + " Estimate Trends", "</tbody>");
            string nextYearBasic = FindSubstringWithBeginAndEnd(wsjHtmlCode, "FY " + nextYear + " Estimate Trends", "</tbody>");

            double currYearCurrent = double.Parse(FindSubstringWithBeginAndEnd(currYearBasic, "</sup>", "</td>"));
            double nextYearCurrent = double.Parse(FindSubstringWithBeginAndEnd(nextYearBasic, "</sup>", "</td>"));

            double result = (nextYearCurrent - currYearCurrent) * 100 / currYearCurrent;

            return result;
        }

        private double CalculateReversalEffect(string stockQuote)
        {
            throw new NotImplementedException();
        }

        private double CalculatePriceMomentum(string stockQuote)
        {
            throw new NotImplementedException();
        }

        private double CalculatePriceChange12months(string stockQuote)
        {
            throw new NotImplementedException();
        }

        private double CalculatePriceChange6months(string stockQuote)
        {
            throw new NotImplementedException();
        }

        private double CalculateProfitRevision(string wsjHtmlCode)
        {
            string currentYear = DateTime.Now.Year.ToString();
            string nextYear = (DateTime.Now.Year + 1).ToString();

            string currYearBasic = FindSubstringWithBeginAndEnd(wsjHtmlCode, "FY " + currentYear + " Estimate Trends", "</tbody>");
            string nextYearBasic = FindSubstringWithBeginAndEnd(wsjHtmlCode, "FY " + nextYear + " Estimate Trends", "</tbody>");

            double currYearCurrent = double.Parse(FindSubstringWithBeginAndEnd(currYearBasic, "</sup>", "</td>"));
            double currYear1monthago = double.Parse(FindSubstringWithBeginAndEnd(currYearBasic.Substring(StringOccurrences(currYearBasic, "</sup>", 2)), "</sup>", "</td>"));
            double nextYearCurrent = double.Parse(FindSubstringWithBeginAndEnd(nextYearBasic, "</sup>", "</td>"));
            double nextYear1monthago = double.Parse(FindSubstringWithBeginAndEnd(nextYearBasic.Substring(StringOccurrences(nextYearBasic, "</sup>", 2)), "</sup>", "</td>"));

            double currYearMonthChange = (currYearCurrent - currYear1monthago) * 100 / currYear1monthago;
            double nextYearMonthChange = (nextYearCurrent - nextYear1monthago) * 100 / nextYear1monthago;
            double result = (currYearMonthChange + nextYearMonthChange) / 2;

            return result;
        }

        private double CalculateReactionToQuarterlyRelease(JObject detailsData)
        {
            // set search company name - only first word
            string shortName = detailsData["price"]["shortName"].Value<string>();
            string firstWord = shortName.Split(' ').First();

            // set from and to dates + formatting
            string from = DateTime.Now.ToString("MM/dd/yyyy");
            string to = DateTime.Now.AddMonths(-3).ToString("MM/dd/yyyy");

            string urlAddress = "https://markets.businessinsider.com/earnings-calendar#date=01/01/2020-03/27/2020&name=" + firstWord + "&countries=&eventtypes=99&tab=ALL";

            // get page source from businessinsider.com
            string htmlCode = GetHtmlCode(urlAddress);

            return 0;
        }

        private double CalculateAnalystOpinions(string wsjHtmlCode)
        {
            try
            {
                string extract1 = FindSubstringWithBeginAndEnd(wsjHtmlCode, "<h3>Analyst Ratings <span class=\"hdr_co_name\">", "<td><span class=\"data_lbl\">Consensus</span></td>");
                string extractBuy = FindSubstringWithBeginAndEnd(extract1, "Buy", "</tr>");
                string extractHold = FindSubstringWithBeginAndEnd(extract1, "Hold", "</tr>");
                string extractSell = FindSubstringWithBeginAndEnd(extract1, "Sell", "</tr>");

                string begin = "<span class=\"data_data\">";
                int lenBegin = begin.Length;
                string end = "</span>";
                int lenEnd = end.Length;

                int buyStart = StringOccurrences(extractBuy, begin, 3);
                string buyCount = FindSubstringWithBeginAndEnd(extractBuy.Substring(buyStart), begin, end);
                var buyCountFinalShit = double.Parse(buyCount, System.Globalization.CultureInfo.InvariantCulture);

                int holdStart = StringOccurrences(extractHold, begin, 3);
                string holdCount = FindSubstringWithBeginAndEnd(extractHold.Substring(holdStart), begin, end);
                var holdCountFinalShit = double.Parse(holdCount, System.Globalization.CultureInfo.InvariantCulture);

                int sellStart = StringOccurrences(extractSell, begin, 3);
                string sellCount = FindSubstringWithBeginAndEnd(extractSell.Substring(sellStart), begin, end);
                var sellCountFinalShit = double.Parse(sellCount, System.Globalization.CultureInfo.InvariantCulture);

                double result = (buyCountFinalShit + holdCountFinalShit * 2 + sellCountFinalShit * 3) / (buyCountFinalShit + holdCountFinalShit + sellCountFinalShit);

                return result;
            }
            catch
            {
                return 0;
            }
        }

        private double CalculatePE5years(JObject detailsData)
        {
            if (detailsData == null)
                return 0;

            // get company short name - best for search
            string shortName = detailsData["price"]["shortName"].Value<string>();

            // leave only alphanumeric characters
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            shortName = rgx.Replace(shortName, "");
            // convert to HTML string to use in URL
            shortName = Uri.EscapeUriString(shortName);

            // search with above name with auto search
            var client = new RestClient("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/auto-complete?lang=en&region=US&query=" + shortName);
            JObject autoCompleteResult = GetData(client);
            string searchQuoteName = autoCompleteResult["ResultSet"]["Result"][0]["symbol"].Value<string>();

            // get page source from ycharts.com
            string urlAddress = "https://ycharts.com/companies/" + searchQuoteName + "/pe_ratio";
            string htmlCode = GetHtmlCode(urlAddress);

            // get P/E 5 years average text
            string extract1 = FindSubstringWithBeginAndEnd(htmlCode, "Average</td>", "/td>");
            string extract2 = FindSubstringWithBeginAndEnd(extract1, "<td class=\"col2\">", "<");
            string finalContent = Regex.Replace(extract2, @"\s+", string.Empty);

            // convert to double
            var result = double.Parse(finalContent, System.Globalization.CultureInfo.InvariantCulture);
            return result / 100; // get percentage
        }

        private double CalculatePE1year(JObject detailsData)
        {
            if (detailsData == null)
                return 0;

            double PE1year = detailsData["summaryDetail"]["trailingPE"]["raw"].Value<double>();
            return PE1year;
        }

        private double CalculateEquityRatio(JObject financialsData)
        {
            if (financialsData == null)
                return 0;

            double stockholderEquity = financialsData["balanceSheetHistory"]["balanceSheetStatements"][0]["totalStockholderEquity"]["raw"].Value<double>();
            double totalAssets = financialsData["balanceSheetHistory"]["balanceSheetStatements"][0]["totalAssets"]["raw"].Value<double>();
            double equityRatio = stockholderEquity / totalAssets;

            return equityRatio;
        }

        private double CalculateEBITMargin(JObject financialsData)
        {
            if (financialsData == null)
                return 0;

            double EBIT = financialsData["incomeStatementHistory"]["incomeStatementHistory"][0]["ebit"]["raw"].Value<double>();
            double totalRevenue = financialsData["incomeStatementHistory"]["incomeStatementHistory"][0]["totalRevenue"]["raw"].Value<double>();
            double EBITMargin = EBIT / totalRevenue;

            return EBITMargin;
        }

        private double CalculateRoE(JObject detailsData)
        {
            if (detailsData == null)
                return 0;

            double RoE = detailsData["financialData"]["returnOnEquity"]["raw"].Value<double>();
            return RoE;
        }

        private JObject GetDetailsData(string stockQuote)
        {
            var client = new RestClient("https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/get-detail?region=US&lang=en&symbol=" + stockQuote);
            return GetData(client);
        }

        private JObject GetFinancialsData(string stockQuote)
        {
            var client = new RestClient("https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-financials?symbol=" + stockQuote);
            return GetData(client);
        }

        private static JObject GetData(RestClient client)
        {
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "apidojo-yahoo-finance-v1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "24af7fd5b2msh3c45c5735b57c13p1dc477jsn00e0e44c4cd5");
            IRestResponse response = client.Execute(request);
            var data = (JObject)JsonConvert.DeserializeObject(response.Content);
            return data;
        }

        private string FindSubstringWithBeginAndEnd(string text, string begin, string end)
        {
            int from = text.IndexOf(begin) + begin.Length;
            int to = text.IndexOf(end, from);
            string result = text.Substring(from, to - from);
            return result;
        }

        private string GetWsjHtmlCode(string stockQuote)
        {
            // check if stock is US or German-XRTRA
            int from = stockQuote.IndexOf(".DE");
            string urlAddress;
            if (from < 0)
            {
                urlAddress = "https://www.wsj.com/market-data/quotes/" + stockQuote + "/research-ratings";
            }
            else
            {
                urlAddress = "https://www.wsj.com/market-data/quotes/XE/XETR/" + stockQuote.Remove(from) + "/research-ratings";
            }

            // get page source from wsj.com
            string htmlCode = GetHtmlCode(urlAddress);

            return htmlCode;
        }

        private int StringOccurrences(string text, string pattern, int n)
        {
            // Loop through n instances of the string 'text'.
            int i = 0;
            int count = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                count++;
                if (count == n)
                {
                    return i;
                }
                else
                {
                    i += pattern.Length;
                }
            }

            return -1;
        }

        private string GetHtmlCode(string urlAddress)
        {
            string htmlCode;
            //using (WebClient webClient = new WebClient())
            //{
            //    htmlCode = webClient.DownloadString(urlAddress);
            //}

            WebBrowser wb = new WebBrowser();
            wb.ScriptErrorsSuppressed = true;
            wb.Navigate(new Uri(urlAddress));
            while (wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            var doc = wb.Document;
            htmlCode = doc.Body.InnerHtml;

            return htmlCode;
        }
    }
}
