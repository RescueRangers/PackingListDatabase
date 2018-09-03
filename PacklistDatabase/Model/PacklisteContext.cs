using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacklistDatabase.Model
{
    public class PacklisteContext : DbContext
    {
        public DbSet<Year> Years { get; set; }
        public DbSet<Month> Months { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Packliste> Packlistes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialWithUsage> MaterialsWithUsage { get; set; }

        public PacklisteContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PacklisteContext, Migrations.Configuration>());
        }
    }
}
