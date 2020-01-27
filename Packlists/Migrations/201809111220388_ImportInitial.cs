namespace Packlists.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class ImportInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImportTransports",
                c => new
                {
                    ImportTransportId = c.Int(nullable: false, identity: true),
                    Sender = c.String(),
                    ImportDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.ImportTransportId);

            CreateTable(
                "dbo.MaterialAmounts",
                c => new
                {
                    MaterialAmountId = c.Int(nullable: false, identity: true),
                    Amount = c.Single(nullable: false),
                    Material_MaterialId = c.Int(),
                    ImportTransport_ImportTransportId = c.Int(),
                })
                .PrimaryKey(t => t.MaterialAmountId)
                .ForeignKey("dbo.Materials", t => t.Material_MaterialId)
                .ForeignKey("dbo.ImportTransports", t => t.ImportTransport_ImportTransportId)
                .Index(t => t.Material_MaterialId)
                .Index(t => t.ImportTransport_ImportTransportId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.MaterialAmounts", "ImportTransport_ImportTransportId", "dbo.ImportTransports");
            DropForeignKey("dbo.MaterialAmounts", "Material_MaterialId", "dbo.Materials");
            DropIndex("dbo.MaterialAmounts", new[] { "ImportTransport_ImportTransportId" });
            DropIndex("dbo.MaterialAmounts", new[] { "Material_MaterialId" });
            DropTable("dbo.MaterialAmounts");
            DropTable("dbo.ImportTransports");
        }
    }
}