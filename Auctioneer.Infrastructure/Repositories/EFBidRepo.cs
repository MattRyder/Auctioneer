using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Auctioneer.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Infrastructure.Repositories
{
    public class EFBidRepo : IRepo<Bid>
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Bid> Entities
        {
            get { return context.Bids; }
        }

        public void Add(Bid entity)
        {
            context.Bids.Add(entity);
        }

        public void Delete(Bid entity)
        {
            context.Bids.Remove(entity);
        }

        public Bid Find(int id)
        {
            return context.Bids.FirstOrDefault(bid => bid.ID == id);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void Update(Bid entity)
        {
            if (context.Entry(entity).State != System.Data.Entity.EntityState.Detached)
                context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public IQueryable<Bid> Where(Expression<Func<Bid, bool>> predicate)
        {
            return context.Bids.Where(predicate);
        }
    }
}
