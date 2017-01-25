namespace Auctioneer.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReservePriceToAuction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Auctions", "MinimumPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Auctions", "MinimumPrice");
        }
    }
}
