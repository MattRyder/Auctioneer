using Auctioneer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Infrastructure.Entities
{
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base() { }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
    }
}
