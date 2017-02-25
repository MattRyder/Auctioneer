using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Auctioneer.Infrastructure.Entities;
using Auctioneer.Infrastructure.SignalR;
using Auctioneer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vereyon.Web;

namespace Auctioneer.Controllers
{
    [System.Web.Mvc.Authorize]
    public class AuctionController : BaseController
    {
        private IRepo<Auction> repo;
        private IHtmlSanitizer sanitizer;
        private EmailServiceBase emailService;
        private AuctioneerUserManager userManager;

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

        public AuctionController(IRepo<Auction> auctionRepo, AuctioneerUserManager userManager, IHtmlSanitizer htmlSanitizer, EmailServiceBase emailService)
        {
            this.repo = auctionRepo;
            this.sanitizer = htmlSanitizer;
            this.userManager = userManager;
            this.emailService = emailService;

        }

        // GET: Auction
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(repo.Entities);
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            Auction auction = repo.Find(id);

            if (auction == null)
                return new HttpNotFoundResult();

            PlaceBidViewModel model = new PlaceBidViewModel()
            {
                Auction = auction,
                Bid = new Bid() { AuctioneerUser_Id = User.Identity.GetUserId() },
                WinningBid = auction.WinningBid()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.DurationSelectList = durationSelectList;
            return View(new Auction() { AuctioneerUser_Id = User.Identity.GetUserId() });
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

            if (auction.MinimumPrice == 0)
                auction.MinimumPrice = 0.01M;

            if (ModelState.IsValid)
            {
                repo.Add(auction);
                repo.SaveChanges();

                SetFlashMessage(FlashKeyType.Success, FlashMessageCreateSuccess);
                return RedirectToAction("Details", new { id = auction.ID });
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
        public async Task<ActionResult> Bid(int id, Bid bid)
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

                BidHub.PushUpdateBidAmount(id.ToString(), bid.Amount);

                var user = userManager.FindById(bid.AuctioneerUser_Id);
                await emailService.SendEmailAsync(user.Email, user.Name, "You've placed a Bid on Auctioneer!", $"Foo Bar. You placed a Bid on \"{bid.Auction.Title}\" for {bid.Amount.ToString("c")}");
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