namespace Auctioneer.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Subtitle = c.String(),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Auction_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Auctions", t => t.Auction_ID)
                .Index(t => t.Auction_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "Auction_ID", "dbo.Auctions");
            DropIndex("dbo.Bids", new[] { "Auction_ID" });
            DropTable("dbo.Bids");
            DropTable("dbo.Auctions");
        }
    }
}
