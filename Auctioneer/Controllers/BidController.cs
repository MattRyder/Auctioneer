using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Controllers
{
    public class BidController : BaseController
    {
        private IRepo<Bid> repo;
        private IRepo<Auction> auctionRepo;

        public BidController(IRepo<Auction> auctionRepo, IRepo<Bid> bidRepo)
        {
            this.auctionRepo = auctionRepo;
            this.repo = bidRepo;
        }

        // GET: Bid
        public ActionResult Index()
        {
            return View(repo);
        }
    }
}