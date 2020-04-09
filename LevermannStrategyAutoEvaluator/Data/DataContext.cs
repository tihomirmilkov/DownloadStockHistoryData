using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevermannStrategyAutoEvaluator.Data
{
    public class DataContext : DbContext
    {
        public DataContext() : base("name=StockEvaluatorDBConnectionString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, LevermannStrategyAutoEvaluator.Migrations.Configuration>());
        }

        public DbSet<LevermannParameter> LevermannParameters { get; set; }
        public DbSet<LevermannFinalPoint> LevermannFinalPoints { get; set; }
        public DbSet<Quote> Quotes { get; set; }
    }
}
