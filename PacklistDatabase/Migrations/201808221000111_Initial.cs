using System.Data.Entity.Migrations;

namespace PacklistDatabase.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Days",
                c => new
                    {
                        DayId = c.Int(nullable: false, identity: true),
                        DayNumber = c.Int(nullable: false),
                        Month_MonthId = c.Int(),
                    })
                .PrimaryKey(t => t.DayId)
                .ForeignKey("dbo.Months", t => t.Month_MonthId)
                .Index(t => t.Month_MonthId);
            
            CreateTable(
                "dbo.Packlistes",
                c => new
                    {
                        PacklisteId = c.Int(nullable: false, identity: true),
                        PacklisteNumber = c.Int(nullable: false),
                        PacklisteHeader = c.String(),
                        Day_DayId = c.Int(),
                    })
                .PrimaryKey(t => t.PacklisteId)
                .ForeignKey("dbo.Days", t => t.Day_DayId)
                .Index(t => t.Day_DayId);
            
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
                        MaterialName = c.String(),
                    })
                .PrimaryKey(t => t.MaterialId);
            
            CreateTable(
                "dbo.Months",
                c => new
                    {
                        MonthId = c.Int(nullable: false, identity: true),
                        MonthNumber = c.Int(nullable: false),
                        MonthName = c.String(),
                        Year_YearId = c.Int(),
                    })
                .PrimaryKey(t => t.MonthId)
                .ForeignKey("dbo.Years", t => t.Year_YearId)
                .Index(t => t.Year_YearId);
            
            CreateTable(
                "dbo.Years",
                c => new
                    {
                        YearId = c.Int(nullable: false, identity: true),
                        YearNumber = c.Int(nullable: false),
                        IsEdited = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.YearId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Months", "Year_YearId", "dbo.Years");
            DropForeignKey("dbo.Days", "Month_MonthId", "dbo.Months");
            DropForeignKey("dbo.Packlistes", "Day_DayId", "dbo.Days");
            DropForeignKey("dbo.Items", "Packliste_PacklisteId", "dbo.Packlistes");
            DropForeignKey("dbo.MaterialWithUsages", "Item_ItemId", "dbo.Items");
            DropForeignKey("dbo.MaterialWithUsages", "Material_MaterialId", "dbo.Materials");
            DropIndex("dbo.Months", new[] { "Year_YearId" });
            DropIndex("dbo.MaterialWithUsages", new[] { "Item_ItemId" });
            DropIndex("dbo.MaterialWithUsages", new[] { "Material_MaterialId" });
            DropIndex("dbo.Items", new[] { "Packliste_PacklisteId" });
            DropIndex("dbo.Packlistes", new[] { "Day_DayId" });
            DropIndex("dbo.Days", new[] { "Month_MonthId" });
            DropTable("dbo.Years");
            DropTable("dbo.Months");
            DropTable("dbo.Materials");
            DropTable("dbo.MaterialWithUsages");
            DropTable("dbo.Items");
            DropTable("dbo.Packlistes");
            DropTable("dbo.Days");
        }
    }
}
