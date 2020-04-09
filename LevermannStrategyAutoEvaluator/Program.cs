using LevermannStrategyAutoEvaluator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevermannStrategyAutoEvaluator
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // need this shit below and [STAThread] in order for the stupid WebBrowser to work
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var evaluator = new TheMotherEvaluator();
            string quoteName = "RRTL.DE"; // "AAPL" // "RRTL.DE" // ADS.DE
            evaluator.EvaluateMotherFuckerr(quoteName, false); 

            Console.WriteLine(evaluator.shortQuoteNameAndPrice);
            Console.WriteLine("1. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.RoE, evaluator.levermannFinalPoints.RoE > 0 ? "+":"", evaluator.levermannFinalPoints.RoE);
            Console.WriteLine("2. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.EBITMargin, evaluator.levermannFinalPoints.EBITMargin > 0 ? "+" : "", evaluator.levermannFinalPoints.EBITMargin);
            Console.WriteLine("3. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.EquityRatio, evaluator.levermannFinalPoints.EquityRatio > 0 ? "+" : "", evaluator.levermannFinalPoints.EquityRatio);
            Console.WriteLine("4. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.PE1year, evaluator.levermannFinalPoints.PE1year > 0 ? "+" : "", evaluator.levermannFinalPoints.PE1year);
            Console.WriteLine("5. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.PE5years, evaluator.levermannFinalPoints.PE5years > 0 ? "+" : "", evaluator.levermannFinalPoints.PE5years);
            Console.WriteLine("6. {0:0.00} {1}{2}", 
                evaluator.levermannParameters.AnalystOpinions, evaluator.levermannFinalPoints.AnalystOpinions > 0 ? "+" : "", evaluator.levermannFinalPoints.AnalystOpinions);
            Console.WriteLine("7. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.ReactionToQuarterlyRelease, evaluator.levermannFinalPoints.ReactionToQuarterlyRelease > 0 ? "+" : "", evaluator.levermannFinalPoints.ReactionToQuarterlyRelease);
            Console.WriteLine("8. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.ProfitRevision, evaluator.levermannFinalPoints.ProfitRevision > 0 ? "+" : "", evaluator.levermannFinalPoints.ProfitRevision);
            Console.WriteLine("9. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.PriceChange6months, evaluator.levermannFinalPoints.PriceChange6months > 0 ? "+" : "", evaluator.levermannFinalPoints.PriceChange6months);
            Console.WriteLine("10. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.PriceChange12months, evaluator.levermannFinalPoints.PriceChange12months > 0 ? "+" : "", evaluator.levermannFinalPoints.PriceChange12months);
            Console.WriteLine("11. {0} {1}{2}", 
                evaluator.levermannParameters.PriceMomentum, evaluator.levermannFinalPoints.PriceMomentum > 0 ? "+" : "", evaluator.levermannFinalPoints.PriceMomentum);
            Console.WriteLine("12. {0} {1}{2}", 
                evaluator.levermannParameters.ReversalEffect, evaluator.levermannFinalPoints.ReversalEffect > 0 ? "+" : "", evaluator.levermannFinalPoints.ReversalEffect);
            Console.WriteLine("13. {0:0.00}% {1}{2}", 
                evaluator.levermannParameters.ProfitGrowth, evaluator.levermannFinalPoints.ProfitGrowth > 0 ? "+" : "", evaluator.levermannFinalPoints.ProfitGrowth);

            int total = evaluator.levermannFinalPoints.GetTotalPoints();
            Console.WriteLine("overall: {0}{1}", total > 0 ? "+" : "", total);

            using (var ctx = new DataContext())
            {
                Quote quote = new Quote();
                quote.Date = DateTime.Now;
                quote.QuoteName = quoteName;
                quote.LevermannFinalPoint = evaluator.levermannFinalPoints;
                quote.LevermannParameter = evaluator.levermannParameters;

                ctx.Quotes.Add(quote);
                ctx.SaveChanges();
            }

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
