namespace Packlists.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PacklisteData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PacklisteDatas",
                c => new
                    {
                        PacklisteDataId = c.Int(nullable: false, identity: true),
                        RowNumber = c.Int(nullable: false),
                        ColumnNumber = c.Int(nullable: false),
                        Data = c.String(),
                        Packliste_PacklisteId = c.Int(),
                    })
                .PrimaryKey(t => t.PacklisteDataId)
                .ForeignKey("dbo.Packlistes", t => t.Packliste_PacklisteId)
                .Index(t => t.Packliste_PacklisteId);
            
            DropColumn("dbo.Packlistes", "PacklisteDataAsJson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Packlistes", "PacklisteDataAsJson", c => c.String());
            DropForeignKey("dbo.PacklisteDatas", "Packliste_PacklisteId", "dbo.Packlistes");
            DropIndex("dbo.PacklisteDatas", new[] { "Packliste_PacklisteId" });
            DropTable("dbo.PacklisteDatas");
        }
    }
}
