using Auctioneer.Core.Entities;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.Owin;

namespace Auctioneer.Infrastructure.Entities
{
    public class AuctioneerSignInManager : SignInManager<AuctioneerUser, string>
    {
        public AuctioneerSignInManager(UserManager<AuctioneerUser, string> userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AuctioneerUser user)
        {
            return user.GenerateUserIdentityAsync((AuctioneerUserManager)UserManager);
        }

        public static AuctioneerSignInManager Create(IdentityFactoryOptions<AuctioneerSignInManager> opts, IOwinContext ctx)
        {
            return new AuctioneerSignInManager(ctx.GetUserManager<AuctioneerUserManager>(), ctx.Authentication);
        }
    }
}
