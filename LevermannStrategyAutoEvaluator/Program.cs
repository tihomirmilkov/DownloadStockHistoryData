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
            evaluator.GetLevermannParameters("RRTL.DE");
            //evaluator.GetLevermannParameters("AAPL");
        }
    }
}
