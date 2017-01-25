namespace Auctioneer.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConstraintToAuction : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Auctions", "Subtitle", c => c.String(maxLength: 140));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Auctions", "Subtitle", c => c.String());
        }
    }
}
