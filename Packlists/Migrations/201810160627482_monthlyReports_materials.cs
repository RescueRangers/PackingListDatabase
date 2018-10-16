namespace Packlists.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class monthlyReports_materials : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Materials", "MonthlyUsageReport_ReportId", "dbo.MonthlyUsageReports");
            DropIndex("dbo.Materials", new[] { "MonthlyUsageReport_ReportId" });
            DropColumn("dbo.Materials", "MonthlyUsageReport_ReportId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Materials", "MonthlyUsageReport_ReportId", c => c.Int());
            CreateIndex("dbo.Materials", "MonthlyUsageReport_ReportId");
            AddForeignKey("dbo.Materials", "MonthlyUsageReport_ReportId", "dbo.MonthlyUsageReports", "ReportId");
        }
    }
}
