namespace LevermannStrategyAutoEvaluator.Data
{
    public class LevermannParameter
    {
        public LevermannParameter()
        {
                
        }

        public int Id { get; set; }
        public double RoE { get; set; }
        public double EBITMargin { get; set; }
        public double EquityRatio { get; set; }
        public double PE1year { get; set; }
        public double PE5years { get; set; }
        public double AnalystOpinions { get; set; }
        public double ReactionToQuarterlyRelease { get; set; }
        public double ProfitRevision { get; set; }
        public double PriceChange6months { get; set; }
        public double PriceChange12months { get; set; }
        public int PriceMomentum { get; set; }
        public int ReversalEffect { get; set; }
        public double ProfitGrowth { get; set; }
    }
}
