namespace LevermannStrategyAutoEvaluator.Data
{
    public class LevermannFinalPoint
    {
        public LevermannFinalPoint()
        {
                
        }

        public int Id { get; set; }
        public int RoE { get; set; }
        public int EBITMargin { get; set; }
        public int EquityRatio { get; set; }
        public int PE1year { get; set; }
        public int PE5years { get; set; }
        public int AnalystOpinions { get; set; }
        public int ReactionToQuarterlyRelease { get; set; }
        public int ProfitRevision { get; set; }
        public int PriceChange6months { get; set; }
        public int PriceChange12months { get; set; }
        public int PriceMomentum { get; set; }
        public int ReversalEffect { get; set; }
        public int ProfitGrowth { get; set; }
        public int TotalPoints { get; set; }

        public int GetTotalPoints()
        {
            int sum = RoE + EBITMargin + EquityRatio + PE1year + PE5years + AnalystOpinions + ReactionToQuarterlyRelease + ProfitRevision 
                + PriceChange6months + PriceChange12months + PriceMomentum + ReversalEffect + ProfitGrowth;

            TotalPoints = sum;

            return sum;
        }
    }
}
