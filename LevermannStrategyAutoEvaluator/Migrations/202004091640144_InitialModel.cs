namespace LevermannStrategyAutoEvaluator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LevermannFinalPoints",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoE = c.Int(nullable: false),
                        EBITMargin = c.Int(nullable: false),
                        EquityRatio = c.Int(nullable: false),
                        PE1year = c.Int(nullable: false),
                        PE5years = c.Int(nullable: false),
                        AnalystOpinions = c.Int(nullable: false),
                        ReactionToQuarterlyRelease = c.Int(nullable: false),
                        ProfitRevision = c.Int(nullable: false),
                        PriceChange6months = c.Int(nullable: false),
                        PriceChange12months = c.Int(nullable: false),
                        PriceMomentum = c.Int(nullable: false),
                        ReversalEffect = c.Int(nullable: false),
                        ProfitGrowth = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LevermannParameters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoE = c.Double(nullable: false),
                        EBITMargin = c.Double(nullable: false),
                        EquityRatio = c.Double(nullable: false),
                        PE1year = c.Double(nullable: false),
                        PE5years = c.Double(nullable: false),
                        AnalystOpinions = c.Double(nullable: false),
                        ReactionToQuarterlyRelease = c.Double(nullable: false),
                        ProfitRevision = c.Double(nullable: false),
                        PriceChange6months = c.Double(nullable: false),
                        PriceChange12months = c.Double(nullable: false),
                        PriceMomentum = c.Int(nullable: false),
                        ReversalEffect = c.Int(nullable: false),
                        ProfitGrowth = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LevermannParameters");
            DropTable("dbo.LevermannFinalPoints");
        }
    }
}
