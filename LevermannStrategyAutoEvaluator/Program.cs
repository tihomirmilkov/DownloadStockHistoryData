using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevermannStrategyAutoEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            var evaluator = new TheMotherEvaluator();
            evaluator.GetLevermannParameters("AAPL");
        }
    }
}
