using Auctioneer.Core.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Auctioneer.Core.Entities;
using System.Linq.Expressions;
using Auctioneer.Infrastructure.Entities;
using System.Threading.Tasks;

namespace Auctioneer.Infrastructure.Repositories
{
    public class EFAuctionRepo : IRepo<Auction>
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Auction> Entities
        {
            get { return context.Auctions; }
        }

        public void Add(Auction entity)
        {
            context.Auctions.Add(entity);
        }

        public void Delete(Auction entity)
        {
            context.Auctions.Remove(entity);
        }

        public Auction Find(int id)
        {
            return context.Auctions.FirstOrDefault(auction => auction.ID == id);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void Update(Auction entity)
        {
            if (context.Entry(entity).State == System.Data.Entity.EntityState.Detached)
                context.Auctions.Attach(entity);

            context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public IQueryable<Auction> Where(Expression<Func<Auction, bool>> predicate)
        {
            return context.Auctions.Where(predicate);
        }
    }
}
