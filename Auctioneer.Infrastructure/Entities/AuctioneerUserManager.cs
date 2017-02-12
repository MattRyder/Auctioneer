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

        /// <summary>
        /// Create an instance of the User Manager
        /// </summary>
        /// <param name="options">Identity options.</param>
        /// <param name="context">Database context.</param>
        /// <returns></returns>
        public static AuctioneerUserManager Create(
            IdentityFactoryOptions<AuctioneerUserManager> options,
            IOwinContext context)
        {
            // Create AppUserManager context from DbContext:
            var manager = new AuctioneerUserManager(
                new UserStore<AuctioneerUser>(context.Get<EFDbContext>()));

            // Username Validation Logic:
            manager.UserValidator = new UserValidator<AuctioneerUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Password Validation Logic:
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // Configure user lockout defaults:
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(10);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Setup Data Protection Provider:
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<AuctioneerUser>(
                    dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }
}