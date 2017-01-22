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

            return View(auction);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Auction());
        }

        [HttpPost]
        public ActionResult Create(Auction auction)
        {
            if (ModelState.IsValid)
            {
                repo.Add(auction);
                repo.SaveChanges();

                SetFlashMessage(FlashKeyType.Success, "Successfully listed your item for auction!");
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
        public ActionResult Edit(Auction auction)
        {
            if (ModelState.IsValid)
            {
                repo.Update(auction);
                repo.SaveChanges();
            }

            return View(auction);
        }


    }
}