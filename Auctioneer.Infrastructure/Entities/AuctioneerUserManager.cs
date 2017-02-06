using Auctioneer.Core.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Infrastructure.Entities
{
    public class AuctioneerUserManager : UserManager<AuctioneerUser>
    {
        public AuctioneerUserManager(IUserStore<AuctioneerUser> store) : base(store) { }

        public static AuctioneerUserManager Create(IdentityFactoryOptions<AuctioneerUserManager> opts, IOwinContext ctx)
        {
            var manager = new AuctioneerUserManager(new UserStore<AuctioneerUser>(ctx.Get<EFDbContext>()));

            manager.UserValidator = new UserValidator<AuctioneerUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
            };

            manager.PasswordValidator = new PasswordValidator()
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = true
            };

            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(10);
            manager.MaxFailedAccessAttemptsBeforeLockout = 3;

            var dpp = opts.DataProtectionProvider;
            if(dpp != null)
                manager.UserTokenProvider = new DataProtectorTokenProvider<AuctioneerUser>(dpp.Create("ASP.NET Identity"));

            return manager;
        }
    }
}
