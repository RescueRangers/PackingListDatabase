namespace Packlists.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class PacklisteDestination : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Packlistes", "Destination", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Packlistes", "Destination");
        }
    }
}
