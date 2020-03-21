using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

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

            // 1. RoE - Return on Equity
            result.RoE = CalculateRoE(detailsData);

            // 2. EBIT margin
            result.EBITMargin = CalculateEBITMargin(financialsData);

            // 3. Equity ratio = total shareholders equity / total assets
            result.EquityRatio = CalculateEquityRatio(financialsData);

            // 4. P/E ratio 1 year
            result.PE1year = CalculatePE1year(detailsData);

            // 5. P/E ratio 5 years
            result.PE5years = CalculatePE5years(stockQuote);

            // 6. Analyst opinions
            result.AnalystOpinions = CalculateAnalystOpinions(stockQuote);

            // 7. Reactions to quarterly figures relase
            result.ReactionToQuarterlyRelease = CalculateReactionToQuarterlyRelease(stockQuote);

            // 8. Profit revision
            // The difference between the analysts’ estimates for earnings per share of 4 weeks ago is compared with the current expectations.
            // This is done for current fiscal year and next fiscal year.
            result.ProfitRevision = CalculateProfitRevision(stockQuote);

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
            result.ProfitGrowth = CalculateProfitGrowth(stockQuote);

            return result;
        }

        private double CalculateProfitGrowth(string stockQuote)
        {
            throw new NotImplementedException();
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

        private double CalculateProfitRevision(string stockQuote)
        {
            throw new NotImplementedException();
        }

        private double CalculateReactionToQuarterlyRelease(string stockQuote)
        {
            throw new NotImplementedException();
        }

        private double CalculateAnalystOpinions(string stockQuote)
        {
            throw new NotImplementedException();
        }

        private double CalculatePE5years(string stockQuote)
        {
            throw new NotImplementedException();
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
    }
}
