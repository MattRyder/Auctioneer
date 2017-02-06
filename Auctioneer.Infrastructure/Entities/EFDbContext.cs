using Auctioneer.Core.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Auctioneer.Infrastructure.Entities
{
    public class EFDbContext : IdentityDbContext<AuctioneerUser>
    {
        public EFDbContext() : base() { }

        public static EFDbContext Create()
        {
            return new EFDbContext();
        }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
    }
}
