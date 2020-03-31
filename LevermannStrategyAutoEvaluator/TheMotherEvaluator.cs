﻿using System;
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
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Reflection;

namespace LevermannStrategyAutoEvaluator
{
    class TheMotherEvaluator : IDisposable
    {
        private readonly IWebDriver _driver;

        public LevermannParameters levermannParameters;
        public LevermannFinalPoints levermannFinalPoints;
        public string shortQuoteNameAndPrice;

        public TheMotherEvaluator()
        {
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            levermannParameters = new LevermannParameters();
            levermannFinalPoints = new LevermannFinalPoints();
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        public void EvaluateMotherFuckerr(string stockQuote)
        {
            levermannParameters = GetLevermannParameters(stockQuote);
            levermannFinalPoints = GetLevermannFinalPoints(levermannParameters);
            shortQuoteNameAndPrice = GetShortQuoteNameAndPrice(stockQuote);
        }

        public string GetShortQuoteNameAndPrice(string stockQuote)
        {
            JObject detailsData = GetDetailsData(stockQuote);
            string shortName = detailsData["price"]["shortName"].Value<string>();
            string price = detailsData["financialData"]["currentPrice"]["fmt"].Value<string>();

            string result = shortName + " (" + stockQuote + ") - " + price;
            return result;
        }

        public LevermannFinalPoints GetLevermannFinalPoints(LevermannParameters levermannParameters)
        {
            double tolerance;
            var result = new LevermannFinalPoints();

            // 1
            // +1, if the return on equity is greater than 20 %.
            // 0, if the return on equity is between 10 and 20 %.
            // - 1, if the return on equity is less than 10 %.
            tolerance = 0.3;
            if (levermannParameters.RoE >= 20 - tolerance)
                result.RoE = 1;
            if (levermannParameters.RoE < 10 - tolerance)
                result.RoE = -1;

            // 2
            // +1, if the EBIT-Margin is greater than 12%.
            // 0, if the EBIT-Margin is between 6 and 12%.
            // -1, if the EBIT-Margin is less than 6%.
            tolerance = 0.2;
            if (levermannParameters.EBITMargin >= 12 - tolerance)
                result.EBITMargin = 1;
            if (levermannParameters.EBITMargin < 6 - tolerance)
                result.EBITMargin = -1;

            // 3
            // +1, if the equity ratio is greater than 25%.
            // 0, if the equity ratio is between 15 and 25%.
            // -1, if the equity ratio is less than 15%.
            tolerance = 0.5;
            if (levermannParameters.EquityRatio >= 25 - tolerance)
                result.EquityRatio = 1;
            if (levermannParameters.EquityRatio < 15 - tolerance)
                result.EquityRatio = -1;

            // 4
            // +1, if the P/E ratio is less than 12, but greater than 0.
            // 0 if the P/E ratio is between 12 and 16.
            // -1, if the P/E ratio is greater than 16 or less than 0.
            tolerance = 0.2;
            if (levermannParameters.PE1year > 0 && levermannParameters.PE1year <= 12 + tolerance)
                result.PE1year = 1;
            if (levermannParameters.PE1year > 16 + tolerance)
                result.PE1year = -1;

            // 5
            if (levermannParameters.PE5years > 0 && levermannParameters.PE5years <= 12 + tolerance)
                result.PE5years = 1;
            if (levermannParameters.PE5years > 16 + tolerance)
                result.PE5years = -1;

            // 6
            // +1, if the factor is greater than or equal to 2,5.
            // 0, if the factor is between 1,5 and 2,5.
            // -1, if the factor is less than or equal to 1,5.
            tolerance = 0.1;
            if (levermannParameters.AnalystOpinions >= 2.5 - tolerance)
                result.AnalystOpinions = 1;
            if (levermannParameters.AnalystOpinions < 1.5 - tolerance)
                result.AnalystOpinions = -1;

            // 7
            // +1, if the adjusted performance is greater than 1%.
            // -1 if the adjusted performance is less than -1%.
            tolerance = 0;
            if (levermannParameters.ReactionToQuarterlyRelease >= 1 - tolerance)
                result.ReactionToQuarterlyRelease = 1;
            if (levermannParameters.ReactionToQuarterlyRelease < -1 - tolerance)
                result.ReactionToQuarterlyRelease = -1;

            // 8
            // +1, if the profit revision is greater than 5%.
            // 0, if the profit revision is between -5% and + 5%.
            // -1 if the profit revision is less than -5%.
            tolerance = 0.1;
            if (levermannParameters.ProfitRevision >= 5 - tolerance)
                result.ProfitRevision = 1;
            if (levermannParameters.ProfitRevision < -5 - tolerance)
                result.ProfitRevision = -1;

            // 9
            // +1, if the price has risen more than + 5% over the period.
            // 0, if the price has changed between -5% and + 5% during the period.
            // -1, if the price has fallen more than -5% over the period.
            tolerance = 0.1;
            if (levermannParameters.PriceChange6months >= 5 - tolerance)
                result.PriceChange6months = 1;
            if (levermannParameters.PriceChange6months < -5 - tolerance)
                result.PriceChange6months = -1;

            // 10
            if (levermannParameters.PriceChange12months >= 5 - tolerance)
                result.PriceChange12months = 1;
            if (levermannParameters.PriceChange12months < -5 - tolerance)
                result.PriceChange12months = -1;

            // 11
            result.PriceMomentum = levermannParameters.PriceMomentum;

            // 12
            result.ReversalEffect = levermannParameters.ReversalEffect;

            // 13
            // +1, if earnings growth is greater than 5%.
            // 0, if earnings growth is between -5% and +5%.
            // -1, if earnings growth is less than -5%.
            tolerance = 0.1;
            if (levermannParameters.ProfitGrowth >= 5 - tolerance)
                result.ProfitGrowth = 1;
            if (levermannParameters.ProfitGrowth < -5 - tolerance)
                result.ProfitGrowth = -1;

            return result;
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
            result.PE5years = CalculatePE5years(stockQuote, detailsData);

            // 6. Analyst opinions
            result.AnalystOpinions = CalculateAnalystOpinions(wsjHtmlCode);

            // 7. Reactions to quarterly figures relase
            result.ReactionToQuarterlyRelease = CalculateReactionToQuarterlyRelease(stockQuote, detailsData);

            // 8. Profit revision
            // The difference between the analysts’ estimates for earnings per share of 4 weeks ago is compared with the current expectations.
            // This is done for current fiscal year and next fiscal year.
            result.ProfitRevision = CalculateProfitRevision(wsjHtmlCode);

            // 9. Price change 6 months
            result.PriceChange6months = CalculatePriceChange6months(stockQuote);

            // 10. Price change 12 months
            result.PriceChange12months = CalculatePriceChange12months(stockQuote);

            // 11. Price momentum
            result.PriceMomentum = CalculatePriceMomentum(result.PriceChange6months, result.PriceChange12months);

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

        private int CalculateReversalEffect(string stockQuote)
        {
            // get Unix Timestamp period
            int checkIntervalBegin = (int)(DateTime.Now.Date.AddMonths(-3).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int checkIntervalEnd = (int)(DateTime.Now.Date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // get stock history
            JObject stockHistoricalData = GetHistoricalData(stockQuote, checkIntervalBegin, checkIntervalEnd);
            // get stock prices
            double stockBefore3months = stockHistoricalData["prices"][stockHistoricalData["prices"].Count() - 1]["close"].Value<double>();
            double stockBefore2months = stockHistoricalData["prices"][stockHistoricalData["prices"].Count() - 31]["close"].Value<double>();
            double stockBefore1month = stockHistoricalData["prices"][stockHistoricalData["prices"].Count() - 61]["close"].Value<double>();
            double stockNow = stockHistoricalData["prices"][0]["close"].Value<double>();
            // calc stock diff
            double stockDiff3 = (stockBefore2months - stockBefore3months) * 100 / stockBefore3months;
            double stockDiff2 = (stockBefore1month - stockBefore2months) * 100 / stockBefore2months;
            double stockDiff1 = (stockNow - stockBefore1month) * 100 / stockBefore1month;

            // get benchmark history
            string benchmarkQuote;
            if (stockQuote.Contains(".DE"))
            {
                // use DAX Index as benchmark index
                benchmarkQuote = "%255EGDAXI"; // ^GDAXI
            }
            else
            {
                // use S&P 500 Index as benchmark index
                benchmarkQuote = "%255EGSPC"; // ^GSPC
            }
            JObject benchmarkHistoricalData = GetHistoricalData(benchmarkQuote, checkIntervalBegin, checkIntervalEnd);
            // get benchmark prices
            double benchmarkBefore3months = benchmarkHistoricalData["prices"][benchmarkHistoricalData["prices"].Count() - 1]["close"].Value<double>();
            double benchmarkBefore2months = benchmarkHistoricalData["prices"][benchmarkHistoricalData["prices"].Count() - 31]["close"].Value<double>();
            double benchmarkBefore1month = benchmarkHistoricalData["prices"][benchmarkHistoricalData["prices"].Count() - 61]["close"].Value<double>();
            double benchmarkNow = benchmarkHistoricalData["prices"][0]["close"].Value<double>();
            // calc benchmark diff
            double benchmarkDiff3 = (benchmarkBefore2months - benchmarkBefore3months) * 100 / benchmarkBefore3months;
            double benchmarkDiff2 = (benchmarkBefore1month - benchmarkBefore2months) * 100 / benchmarkBefore2months;
            double benchmarkDiff1 = (benchmarkNow - benchmarkBefore1month) * 100 / benchmarkBefore1month;

            if (stockDiff3 > benchmarkDiff3 && stockDiff2 > benchmarkDiff2 && stockDiff1 > benchmarkDiff1)
            {
                return -1;
            }

            if (stockDiff3 < benchmarkDiff3 && stockDiff2 < benchmarkDiff2 && stockDiff1 < benchmarkDiff1)
            {
                return 1;
            }

            return 0;
        }

        private int CalculatePriceMomentum(double priceChange6months, double priceChange12months)
        {
            // +1, if factor 9 = +1 and factor 10 = 0 or -1
            if (priceChange6months >= 5 && priceChange12months >= -5 && priceChange12months <= 5)
            {
                return 1;
            }

            // -1, if factor 9 = -1 and factor 10 = 0 or +1
            if (priceChange6months <= -5 && priceChange12months >= -5 && priceChange12months <= 5)
            {
                return -1;
            }

            // 0, in all other cases
            return 0;
        }

        private double CalculatePriceChange12months(string stockQuote)
        {
            return CalculatePriceChange(stockQuote, 12);
        }

        private double CalculatePriceChange6months(string stockQuote)
        {
            return CalculatePriceChange(stockQuote, 6);
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

        private double CalculateReactionToQuarterlyRelease(string stockQuote, JObject detailsData)
        {
            // TODO Maybe try first if yahoo finance has the data - https://finance.yahoo.com/calendar/earnings/?symbol=AAPL

            // set search company name - only first word
            string shortName = detailsData["price"]["shortName"].Value<string>();
            string firstWord = shortName.Split(' ').First();
            // leave only alphanumeric characters
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            firstWord = rgx.Replace(firstWord, "");

            // set from and to dates + formatting
            string from = DateTime.Now.AddMonths(-3).ToString("MM/dd/yyyy");
            string to = DateTime.Now.ToString("MM/dd/yyyy");

            string urlAddress = "https://markets.businessinsider.com/earnings-calendar#date=" + from + "-" + to + "&name=" + firstWord + "&countries=&eventtypes=99&tab=ALL";

            // get page source from businessinsider.com
            string htmlCode = GetHtmlCode(urlAddress);

            // get last Quaerterly release date
            string extract1 = FindSubstringWithBeginAndEnd(htmlCode, "<table class=\"table instruments calendar", "/td>");
            string extract2 = FindSubstringWithBeginAndEnd(extract1, "<td>", "<");
            string finalContent = Regex.Replace(extract2, @"\s+", string.Empty);

            // get date year
            int finalContentMonth = int.Parse(finalContent.Substring(0, finalContent.IndexOf("/")));
            int currMonth = DateTime.Now.Month;
            int currYear = DateTime.Now.Year;

            // concat string to get full date
            int finalContentDay = int.Parse(finalContent.Substring(finalContent.IndexOf("/") + 1));
            DateTime quarterlyRleaseDate;
            if (currMonth >= finalContentMonth)
            {
                quarterlyRleaseDate = new DateTime(currYear, finalContentMonth, finalContentDay);
            }
            else
            {
                quarterlyRleaseDate = new DateTime(currYear - 1, finalContentMonth, finalContentDay);
            }

            // get Unix Timestamp period - keep in mind the fucking weekends :)
            int checkIntervalBegin = (int)(quarterlyRleaseDate.AddDays(-2).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int checkIntervalEnd = (int)(quarterlyRleaseDate.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // get stock history
            JObject stockHistoricalData = GetHistoricalData(stockQuote, checkIntervalBegin, checkIntervalEnd);

            // get benchmark index change
            JObject benchmarkHistoricalData;
            if (stockQuote.Contains(".DE"))
            {
                // use DAX Index as benchmark index
                benchmarkHistoricalData = GetHistoricalData("%255EGDAXI", checkIntervalBegin, checkIntervalEnd); // ^GDAXI
            }
            else
            {
                // use S&P 500 Index as benchmark index
                benchmarkHistoricalData = GetHistoricalData("%255EGSPC", checkIntervalBegin, checkIntervalEnd); // ^GSPC
            }

            // finally maaaaaaaaan - get close values for the release date and previous close
            // prices[0].close
            double stockReleaseDateClose = stockHistoricalData["prices"][0]["close"].Value<double>();
            double stockReleaseDatePreviousClose = stockHistoricalData["prices"][1]["close"].Value<double>();
            double benchmarkReleaseDateClose = benchmarkHistoricalData["prices"][0]["close"].Value<double>();
            double benchmarkReleaseDatePreviousClose = benchmarkHistoricalData["prices"][1]["close"].Value<double>();

            // tutto finito - calculate differance
            double stockDifferance = (stockReleaseDateClose - stockReleaseDatePreviousClose) / stockReleaseDatePreviousClose;
            double benchmarkDifferance = (benchmarkReleaseDateClose - benchmarkReleaseDatePreviousClose) / benchmarkReleaseDatePreviousClose;
            double stockAndBenchmarkReleaseDatePerformanceDifferanceFAAAAK = stockDifferance - benchmarkDifferance;

            return stockAndBenchmarkReleaseDatePerformanceDifferanceFAAAAK * 100;
        }

        private double CalculateAnalystOpinions(string wsjHtmlCode)
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

        private double CalculatePE5years(string stockQuote, JObject detailsData)
        {
            if (detailsData == null)
                return 0;

            // get company short name - best for search
            string shortName = detailsData["price"]["shortName"].Value<string>();

            // leave only alphanumeric characters and max 2 words
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            shortName = rgx.Replace(shortName, "");
            int secondWordStart = StringOccurrences(shortName, " ", 2);
            if (secondWordStart > 0)
            {
                shortName = shortName.Substring(0, secondWordStart);
            }
            // convert to HTML string to use in URL
            shortName = Uri.EscapeUriString(shortName);

            // search with above name with auto search
            var client = new RestClient("https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/auto-complete?lang=en&region=US&query=" + shortName);
            JObject autoCompleteResult = GetData(client);
            string searchQuoteName = autoCompleteResult["ResultSet"]["Result"][0]["symbol"].Value<string>();


            // get P/E 5 years average text
            string urlAddress;
            string htmlCode;
            string finalContent = "";
            try
            {
                // get page source from ycharts.com
                urlAddress = "https://ycharts.com/companies/" + searchQuoteName + "/pe_ratio";
                htmlCode = GetHtmlCode(urlAddress);

                htmlCode = FindSubstringWithBeginAndEnd(htmlCode, "Average</td>", "/td>");
                htmlCode = FindSubstringWithBeginAndEnd(htmlCode, "<td class=\"col2\">", "<");
                finalContent = Regex.Replace(htmlCode, @"\s+", string.Empty);
            }
            catch // if nothing is found on the page, try with quote name :)
            {
                // get page source from ycharts.com
                urlAddress = "https://ycharts.com/companies/" + stockQuote + "/pe_ratio";
                htmlCode = GetHtmlCode(urlAddress);

                htmlCode = FindSubstringWithBeginAndEnd(htmlCode, "Average</td>", "/td>");
                htmlCode = FindSubstringWithBeginAndEnd(htmlCode, "<td class=\"col2\">", "<");
                finalContent = Regex.Replace(htmlCode, @"\s+", string.Empty);
            }

            // convert to double
            var result = double.Parse(finalContent, System.Globalization.CultureInfo.InvariantCulture);
            return result;
        }

        private double CalculatePE1year(JObject detailsData)
        {
            if (detailsData == null)
                return 0;

            try
            {
                double PE1year = detailsData["summaryDetail"]["trailingPE"]["raw"].Value<double>();
                return PE1year;
            }
            catch
            {
                return 0;
            }
        }

        private double CalculateEquityRatio(JObject financialsData)
        {
            if (financialsData == null)
                return 0;

            double stockholderEquity = financialsData["balanceSheetHistory"]["balanceSheetStatements"][0]["totalStockholderEquity"]["raw"].Value<double>();
            double totalAssets = financialsData["balanceSheetHistory"]["balanceSheetStatements"][0]["totalAssets"]["raw"].Value<double>();
            double equityRatio = stockholderEquity * 100 / totalAssets;

            return equityRatio;
        }

        private double CalculateEBITMargin(JObject financialsData)
        {
            if (financialsData == null)
                return 0;

            double EBIT = financialsData["incomeStatementHistory"]["incomeStatementHistory"][0]["ebit"]["raw"].Value<double>();
            double totalRevenue = financialsData["incomeStatementHistory"]["incomeStatementHistory"][0]["totalRevenue"]["raw"].Value<double>();
            double EBITMargin = EBIT * 100 / totalRevenue;

            return EBITMargin;
        }

        private double CalculateRoE(JObject detailsData)
        {
            if (detailsData == null)
                return 0;

            double RoE = detailsData["financialData"]["returnOnEquity"]["raw"].Value<double>();
            return RoE * 100;
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

        private JObject GetHistoricalData(string stockQuote, int begin, int end)
        {
            var client = new RestClient("https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-historical-data?frequency=1d&filter=history&period1=" 
                                        + begin.ToString()
                                        + "&period2="
                                        + end.ToString()
                                        + "&symbol=" 
                                        + stockQuote);
            return GetData(client);
        }

        private JObject GetData(RestClient client)
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

        private string GetHtmlCode1(string urlAddress)
        {
            string htmlCode;

            using (WebClient webClient = new WebClient())
            {
                htmlCode = webClient.DownloadString(urlAddress);
            }

            return htmlCode;
        }

        private string GetHtmlCode2(string urlAddress)
        {
            string htmlCode;

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

        private string GetHtmlCode(string urlAddress)
        {
            string htmlCode;

            _driver.Navigate().GoToUrl(urlAddress);
            Thread.Sleep(2000);
            htmlCode = _driver.PageSource;

            return htmlCode;
        }

        private double CalculatePriceChange(string stockQuote, int monthsBefore)
        {
            // get Unix Timestamp period
            int checkIntervalBegin = (int)(DateTime.Now.Date.AddMonths(-monthsBefore).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int checkIntervalEnd = (int)(DateTime.Now.Date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // get stock history
            JObject stockHistoricalData = GetHistoricalData(stockQuote, checkIntervalBegin, checkIntervalEnd);

            // get prices
            double before = stockHistoricalData["prices"][stockHistoricalData["prices"].Count() - 1]["close"].Value<double>();
            double now = stockHistoricalData["prices"][0]["close"].Value<double>();

            // calc diff
            double diff = (now - before) * 100 / before;

            return diff;
        }
    }
}
