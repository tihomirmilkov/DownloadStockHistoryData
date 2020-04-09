using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevermannStrategyAutoEvaluator.Data
{
    public class Quote
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string QuoteName { get; set; }
        public LevermannFinalPoint LevermannFinalPoint { get; set; }
        public LevermannParameter LevermannParameter { get; set; }
    }
}
