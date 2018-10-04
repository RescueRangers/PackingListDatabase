namespace Packlists.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cocs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.COCs",
                c => new
                    {
                        CocId = c.Int(nullable: false, identity: true),
                        InventoryDate = c.DateTime(nullable: false),
                        CocNumber = c.Int(nullable: false),
                        Item_ItemWithQtyId = c.Int(),
                    })
                .PrimaryKey(t => t.CocId)
                .ForeignKey("dbo.ItemWithQties", t => t.Item_ItemWithQtyId)
                .Index(t => t.Item_ItemWithQtyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.COCs", "Item_ItemWithQtyId", "dbo.ItemWithQties");
            DropIndex("dbo.COCs", new[] { "Item_ItemWithQtyId" });
            DropTable("dbo.COCs");
        }
    }
}
