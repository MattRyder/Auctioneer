namespace Auctioneer.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignFieldMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bids", "Auction_ID", "dbo.Auctions");
            DropIndex("dbo.Bids", new[] { "Auction_ID" });
            AlterColumn("dbo.Bids", "Auction_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.Bids", "Auction_ID");
            AddForeignKey("dbo.Bids", "Auction_ID", "dbo.Auctions", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "Auction_ID", "dbo.Auctions");
            DropIndex("dbo.Bids", new[] { "Auction_ID" });
            AlterColumn("dbo.Bids", "Auction_ID", c => c.Int());
            CreateIndex("dbo.Bids", "Auction_ID");
            AddForeignKey("dbo.Bids", "Auction_ID", "dbo.Auctions", "ID");
        }
    }
}
