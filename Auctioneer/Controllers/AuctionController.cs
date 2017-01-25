using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Auctioneer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Auctioneer.Controllers
{
    public class AuctionController : BaseController
    {
        private IRepo<Auction> repo;

        public AuctionController(IRepo<Auction> auctionRepo)
        {
            this.repo = auctionRepo;
        }

        // GET: Auction
        public ActionResult Index()
        {
            return View(repo.Entities);
        }

        public ActionResult Details(int id)
        {
            Auction auction = repo.Find(id);

            if (auction == null)
                return new HttpNotFoundResult();

            PlaceBidViewModel model = new PlaceBidViewModel()
            {
                Auction = auction,
                Bid = new Bid(),
                WinningBid = auction.WinningBid()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Auction());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Auction auction)
        {
            if (ModelState.IsValid)
            {
                repo.Add(auction);
                repo.SaveChanges();

                SetFlashMessage(FlashKeyType.Success, "Successfully listed your item for auction!");
                return RedirectToAction("Index");
            }

            SetFlashMessage(FlashKeyType.Danger, "Failed to create your auction, please review the errors below");
            return View(auction);
        }

        public ActionResult Edit(int id)
        {
            Auction auction = repo.Find(id);

            if (auction == null)
                return new HttpNotFoundResult();

            return View(auction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Auction auction)
        {
            if (ModelState.IsValid)
            {
                repo.Update(auction);
                repo.SaveChanges();

                SetFlashMessage(FlashKeyType.Success, "Successfully updated your auction. Changes will be reflected on the site immediately.");
                return RedirectToAction("Details", new { id = auction.ID });
            } else
            {
                SetFlashMessage(FlashKeyType.Danger, "Failed to update your auction, please check errors and try again.");
                return View(auction);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Auction auction = repo.Find(id);

            if (auction == null)
                return new HttpNotFoundResult();

            repo.Delete(auction);
            repo.SaveChanges();

            SetFlashMessage(FlashKeyType.Success, "Successfully delisted and removed your Auction");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bid(int id, Bid bid)
        {
            Auction auction = repo.Find(id);

            if (auction.MinimumPrice > 0 && bid.Amount <= auction.MinimumPrice)
                ModelState.AddModelError("", $"Bid must be more than the reserved price of {auction.MinimumPrice.ToString("c")}");
            else if (bid.Amount <= auction.WinningBid().Amount)
                ModelState.AddModelError("", $"Bid must be more than the current bid of {auction.WinningBid().Amount.ToString("c")}");

            if (ModelState.IsValid)
            {
                auction.Bids.Add(bid);
                repo.SaveChanges();
            }

            PlaceBidViewModel viewModel = new PlaceBidViewModel()
            {
                Auction = auction,
                Bid = new Bid() { Amount = 0.0M },
                WinningBid = auction.WinningBid()
            };

            return PartialView("BidPlacePartial", viewModel);
        }
    }
}