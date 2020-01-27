namespace Packlists.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class empty : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MaterialWithUsages", "Material_MaterialId", "dbo.Materials");
            DropIndex("dbo.MaterialWithUsages", new[] { "Material_MaterialId" });
            AddColumn("dbo.MaterialAmounts", "Packliste_PacklisteId", c => c.Int());
            CreateIndex("dbo.MaterialAmounts", "Packliste_PacklisteId");
            AddForeignKey("dbo.MaterialAmounts", "Packliste_PacklisteId", "dbo.Packlistes", "PacklisteId");
            DropTable("dbo.MaterialWithUsages");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.MaterialWithUsages",
                c => new
                {
                    MaterialWithUsageId = c.Int(nullable: false, identity: true),
                    Usage = c.Single(nullable: false),
                    Material_MaterialId = c.Int(),
                })
                .PrimaryKey(t => t.MaterialWithUsageId);

            DropForeignKey("dbo.MaterialAmounts", "Packliste_PacklisteId", "dbo.Packlistes");
            DropIndex("dbo.MaterialAmounts", new[] { "Packliste_PacklisteId" });
            DropColumn("dbo.MaterialAmounts", "Packliste_PacklisteId");
            CreateIndex("dbo.MaterialWithUsages", "Material_MaterialId");
            AddForeignKey("dbo.MaterialWithUsages", "Material_MaterialId", "dbo.Materials", "MaterialId");
        }
    }
}