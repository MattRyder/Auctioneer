namespace Auctioneer.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEndDateToAuction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "EndDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "EndDate");
        }
    }
}
