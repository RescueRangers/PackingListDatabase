namespace Packlists.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PacklisteItemsChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Items", "Packliste_PacklisteId", "dbo.Packlistes");
            DropIndex("dbo.Items", new[] { "Packliste_PacklisteId" });
            CreateTable(
                "dbo.ItemWithQties",
                c => new
                {
                    ItemWithQtyId = c.Int(nullable: false, identity: true),
                    Quantity = c.Single(nullable: false),
                    Item_ItemId = c.Int(),
                    Packliste_PacklisteId = c.Int(),
                })
                .PrimaryKey(t => t.ItemWithQtyId)
                .ForeignKey("dbo.Items", t => t.Item_ItemId)
                .ForeignKey("dbo.Packlistes", t => t.Packliste_PacklisteId)
                .Index(t => t.Item_ItemId)
                .Index(t => t.Packliste_PacklisteId);

            DropColumn("dbo.Items", "Quantity");
            DropColumn("dbo.Items", "Packliste_PacklisteId");
        }

        public override void Down()
        {
            AddColumn("dbo.Items", "Packliste_PacklisteId", c => c.Int());
            AddColumn("dbo.Items", "Quantity", c => c.Single(nullable: false));
            DropForeignKey("dbo.ItemWithQties", "Packliste_PacklisteId", "dbo.Packlistes");
            DropForeignKey("dbo.ItemWithQties", "Item_ItemId", "dbo.Items");
            DropIndex("dbo.ItemWithQties", new[] { "Packliste_PacklisteId" });
            DropIndex("dbo.ItemWithQties", new[] { "Item_ItemId" });
            DropTable("dbo.ItemWithQties");
            CreateIndex("dbo.Items", "Packliste_PacklisteId");
            AddForeignKey("dbo.Items", "Packliste_PacklisteId", "dbo.Packlistes", "PacklisteId");
        }
    }
}