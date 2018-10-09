namespace Packlists.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaterialAmount : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.MaterialWithUsages", new[] { "Item_ItemId" });
            AddColumn("dbo.MaterialAmounts", "Item_ItemId", c => c.Int());
            CreateIndex("dbo.MaterialAmounts", "Item_ItemId");
            DropColumn("dbo.MaterialWithUsages", "Item_ItemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MaterialWithUsages", "Item_ItemId", c => c.Int());
            DropIndex("dbo.MaterialAmounts", new[] { "Item_ItemId" });
            DropColumn("dbo.MaterialAmounts", "Item_ItemId");
            CreateIndex("dbo.MaterialWithUsages", "Item_ItemId");
        }
    }
}
