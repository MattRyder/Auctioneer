using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Auctioneer.Core.Entities
{
    public class AuctioneerUser : IdentityUser
    {
        public string Name { get; set; }

        public override string Email { get; set; }

        public virtual ICollection<Auction> Auctions { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AuctioneerUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}
