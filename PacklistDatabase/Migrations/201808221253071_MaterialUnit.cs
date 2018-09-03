namespace PacklistDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaterialUnit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Materials", "Unit", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Materials", "Unit");
        }
    }
}
