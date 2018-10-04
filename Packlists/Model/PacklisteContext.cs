using System.Data.Entity;

namespace Packlists.Model
{
    public sealed class PacklisteContext : DbContext
    {
        //public DbSet<Year> Years { get; set; }
        //public DbSet<Month> Months { get; set; }
        //public DbSet<Day> Days { get; set; }
        public DbSet<Packliste> Packlistes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialWithUsage> MaterialsWithUsage { get; set; }
        public DbSet<ItemWithQty> ItemWithQties { get; set; }
        public DbSet<MaterialAmount> MaterialAmounts { get; set; }
        public DbSet<ImportTransport> ImportTransports { get; set; }
        public DbSet<COC> Cocs { get; set; }

        public PacklisteContext()
        {
            var modelBuilder = new DbModelBuilder();
            //modelBuilder.Entity<Year>().HasMany<Month>(m => m.Months).WithOptional().WillCascadeOnDelete(true);
            //modelBuilder.Entity<Month>().HasMany<Day>(d => d.Days).WithOptional().WillCascadeOnDelete(true);
            //modelBuilder.Entity<Day>().HasMany(p => p.Packlists).WithOptional().WillCascadeOnDelete(true);
            modelBuilder.Entity<Packliste>().HasMany(i => i.ItemsWithQties).WithRequired().WillCascadeOnDelete(false);
            modelBuilder.Entity<MaterialWithUsage>().HasRequired(m => m.Material);
            modelBuilder.Entity<Item>().HasMany(m => m.Materials).WithOptional().WillCascadeOnDelete(true);
            modelBuilder.Entity<ItemWithQty>().HasRequired(i => i.Item).WithRequiredDependent().WillCascadeOnDelete(true);
            modelBuilder.Entity<MaterialAmount>().HasRequired(m => m.Material).WithOptional().WillCascadeOnDelete(true);
            modelBuilder.Entity<ImportTransport>().HasMany(m => m.ImportedMaterials).WithOptional().WillCascadeOnDelete(true);
            modelBuilder.Entity<COC>().HasRequired(i => i.Item).WithRequiredDependent().WillCascadeOnDelete(true);

            OnModelCreating(modelBuilder);

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<PacklisteContext, Migrations.Configuration>());
            //Configuration.AutoDetectChangesEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }
    }
}
