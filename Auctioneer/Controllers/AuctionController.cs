using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Auctioneer.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vereyon.Web;

namespace Auctioneer.Controllers
{
    public class AuctionController : BaseController
    {
        private IRepo<Auction> repo;
        private IHtmlSanitizer sanitizer;

        public static readonly string FlashMessageCreateSuccess = "Successfully listed your item for auction!";
        public static readonly string FlashMessageCreateFailure = "Failed to create your auction, please review the errors below";
        public static readonly string FlashMessageUpdateSuccess = "Successfully updated your auction. Changes will be reflected on the site immediately.";
        public static readonly string FlashMessageUpdateFailure = "Failed to update your auction, please check errors and try again.";
        public static readonly string FlashMessageDeleteSuccess = "Successfully delisted and removed your Auction";

        SelectList durationSelectList = new SelectList(new List<SelectListItem>()
        {
            new SelectListItem() { Text = "One Day", Value = "1" },
            new SelectListItem() { Text = "Three Days", Value = "3" },
            new SelectListItem() { Text = "Five Days", Value = "5" },
            new SelectListItem() { Text = "Seven Days", Value = "7" }
        }, "Value", "Text");

        public AuctionController(IRepo<Auction> auctionRepo, IHtmlSanitizer htmlSanitizer)
        {
            this.repo = auctionRepo;
            this.sanitizer = htmlSanitizer;
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
            ViewBag.DurationSelectList = durationSelectList;
            return View(new Auction());
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Auction auction, int duration)
        {
            // Ensure the description doesn't contain anything but the whitelisted HTML tags:
            auction.Description = sanitizer.Sanitize(auction.Description);

            duration = (duration > 0 || duration <= 7) ? duration : 7;
            auction.EndDate = DateTime.Now.AddDays(duration);
            auction.AuctioneerUser_Id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                repo.Add(auction);
                repo.SaveChanges();

                SetFlashMessage(FlashKeyType.Success, FlashMessageCreateSuccess);
                return RedirectToAction("Index");
            }

            SetFlashMessage(FlashKeyType.Danger, FlashMessageCreateFailure);
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
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Auction auction)
        {
            // Ensure the description doesn't contain anything but the whitelisted HTML tags:
            auction.Description = sanitizer.Sanitize(auction.Description);

            if (ModelState.IsValid)
            {
                repo.Update(auction);
                repo.SaveChanges();

                SetFlashMessage(FlashKeyType.Success, FlashMessageUpdateSuccess);
                return RedirectToAction("Details", new { id = auction.ID });
            } else
            {
                SetFlashMessage(FlashKeyType.Danger, FlashMessageUpdateFailure);
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

            SetFlashMessage(FlashKeyType.Success, FlashMessageDeleteSuccess);
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

            bid.AuctioneerUser_Id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                auction.Bids.Add(bid);
                repo.SaveChanges();
            }

            PlaceBidViewModel viewModel = new PlaceBidViewModel()
            {
                Auction = auction,
                Bid = new Bid(),
                WinningBid = auction.WinningBid()
            };

            return PartialView("BidPlacePartial", viewModel);
        }
    }
}