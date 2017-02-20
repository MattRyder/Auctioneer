using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Auctioneer.Infrastructure.Entities;
using Auctioneer.Infrastructure.Repositories;
using Auctioneer.Infrastructure.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Vereyon.Web;

namespace Auctioneer.Infrastructure
{
    /// <summary>
    /// Handles all Auctioneer DI needs
    /// </summary>
    public class AuctioneerDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        
        public AuctioneerDependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
            RegisterServices();
        } 

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void RegisterServices()
        {
            // Authentication, User Store bindings
            kernel.Bind<IUserStore<AuctioneerUser, string>>().To<UserStore<AuctioneerUser>>();
            kernel.Bind<IUserStore<AuctioneerUser>>().To<UserStore<AuctioneerUser>>();
            kernel.Bind<UserManager<AuctioneerUser, string>>().ToSelf();


            kernel.Bind<IAuthenticationManager>().ToMethod(c => HttpContext.Current.GetOwinContext().Authentication).InRequestScope();
            kernel.Bind<AuctioneerUserManager>().ToMethod(c => HttpContext.Current.GetOwinContext().GetUserManager<AuctioneerUserManager>()).InRequestScope();

            // Domain model bindings
            kernel.Bind<IRepo<Auction>>().To<EFAuctionRepo>();
            kernel.Bind<IRepo<Bid>>().To<EFBidRepo>();

            // HTML sanitizer binding
            kernel.Bind<IHtmlSanitizer>().To<AuctioneerHtmlSanitizer>();

            var jsonSerializerSettings = new JsonSerializer();
            jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            kernel.Bind<JsonSerializer>().ToMethod(context => jsonSerializerSettings);

            kernel.Bind<EmailServiceBase>().ToConstructor(c => new AuctioneerEmailService(
                ConfigurationManager.AppSettings["SmtpHost"],
                ConfigurationManager.AppSettings["SmtpPort"],
                ConfigurationManager.AppSettings["SmtpUser"],
                ConfigurationManager.AppSettings["SmtpPass"]));
        }
    }
}