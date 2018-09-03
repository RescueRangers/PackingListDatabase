namespace Packlists.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        ItemName = c.String(),
                        Quantity = c.Single(nullable: false),
                        Packliste_PacklisteId = c.Int(),
                    })
                .PrimaryKey(t => t.ItemId)
                .ForeignKey("dbo.Packlistes", t => t.Packliste_PacklisteId)
                .Index(t => t.Packliste_PacklisteId);
            
            CreateTable(
                "dbo.MaterialWithUsages",
                c => new
                    {
                        MaterialWithUsageId = c.Int(nullable: false, identity: true),
                        Usage = c.Single(nullable: false),
                        Material_MaterialId = c.Int(),
                        Item_ItemId = c.Int(),
                    })
                .PrimaryKey(t => t.MaterialWithUsageId)
                .ForeignKey("dbo.Materials", t => t.Material_MaterialId)
                .ForeignKey("dbo.Items", t => t.Item_ItemId)
                .Index(t => t.Material_MaterialId)
                .Index(t => t.Item_ItemId);
            
            CreateTable(
                "dbo.Materials",
                c => new
                    {
                        MaterialId = c.Int(nullable: false, identity: true),
                        Unit = c.String(),
                        MaterialName = c.String(),
                    })
                .PrimaryKey(t => t.MaterialId);
            
            CreateTable(
                "dbo.Packlistes",
                c => new
                    {
                        PacklisteId = c.Int(nullable: false, identity: true),
                        PacklisteDate = c.DateTime(nullable: false),
                        PacklisteNumber = c.Int(nullable: false),
                        PacklisteDataAsJson = c.String(),
                    })
                .PrimaryKey(t => t.PacklisteId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "Packliste_PacklisteId", "dbo.Packlistes");
            DropForeignKey("dbo.MaterialWithUsages", "Item_ItemId", "dbo.Items");
            DropForeignKey("dbo.MaterialWithUsages", "Material_MaterialId", "dbo.Materials");
            DropIndex("dbo.MaterialWithUsages", new[] { "Item_ItemId" });
            DropIndex("dbo.MaterialWithUsages", new[] { "Material_MaterialId" });
            DropIndex("dbo.Items", new[] { "Packliste_PacklisteId" });
            DropTable("dbo.Packlistes");
            DropTable("dbo.Materials");
            DropTable("dbo.MaterialWithUsages");
            DropTable("dbo.Items");
        }
    }
}
