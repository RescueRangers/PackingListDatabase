namespace Packlists.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class monthlyReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MonthlyUsageReports",
                c => new
                    {
                        ReportId = c.Int(nullable: false, identity: true),
                        ReportDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ReportId);
            
            CreateTable(
                "dbo.Days",
                c => new
                    {
                        DayId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        MonthlyUsageReport_ReportId = c.Int(),
                    })
                .PrimaryKey(t => t.DayId)
                .ForeignKey("dbo.MonthlyUsageReports", t => t.MonthlyUsageReport_ReportId)
                .Index(t => t.MonthlyUsageReport_ReportId);
            
            AddColumn("dbo.MaterialAmounts", "Day_DayId", c => c.Int());
            AddColumn("dbo.Materials", "MonthlyUsageReport_ReportId", c => c.Int());
            CreateIndex("dbo.MaterialAmounts", "Day_DayId");
            CreateIndex("dbo.Materials", "MonthlyUsageReport_ReportId");
            AddForeignKey("dbo.MaterialAmounts", "Day_DayId", "dbo.Days", "DayId");
            AddForeignKey("dbo.Materials", "MonthlyUsageReport_ReportId", "dbo.MonthlyUsageReports", "ReportId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Materials", "MonthlyUsageReport_ReportId", "dbo.MonthlyUsageReports");
            DropForeignKey("dbo.Days", "MonthlyUsageReport_ReportId", "dbo.MonthlyUsageReports");
            DropForeignKey("dbo.MaterialAmounts", "Day_DayId", "dbo.Days");
            DropIndex("dbo.Days", new[] { "MonthlyUsageReport_ReportId" });
            DropIndex("dbo.Materials", new[] { "MonthlyUsageReport_ReportId" });
            DropIndex("dbo.MaterialAmounts", new[] { "Day_DayId" });
            DropColumn("dbo.Materials", "MonthlyUsageReport_ReportId");
            DropColumn("dbo.MaterialAmounts", "Day_DayId");
            DropTable("dbo.Days");
            DropTable("dbo.MonthlyUsageReports");
        }
    }
}
