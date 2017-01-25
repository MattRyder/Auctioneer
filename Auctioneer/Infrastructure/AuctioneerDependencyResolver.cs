using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Auctioneer.Infrastructure.Repositories;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            kernel.Bind<IRepo<Auction>>().To<EFAuctionRepo>();
            kernel.Bind<IRepo<Bid>>().To<EFBidRepo>();
        }
    }
}