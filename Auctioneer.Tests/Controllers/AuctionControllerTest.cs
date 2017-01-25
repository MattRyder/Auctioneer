﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Moq;
using System.Collections.Generic;
using Auctioneer.Controllers;
using System.Web.Mvc;
using System.Linq;
using Auctioneer.Models;

namespace Auctioneer.Tests.Controllers
{
    [TestClass]
    public class AuctionControllerTest
    {
        private static List<Auction> auctionData;
        private static Mock<IRepo<Auction>> auctionRepo;

        [TestInitialize]
        public void TestInitialize()
        {
            auctionData = new List<Auction>()
            {
                new Auction() { ID = 1, Title = "Auction A", Subtitle = "Auction A Subtitle", Description = "Auction A Description", MinimumPrice = 12.50M, Bids = new List<Bid>() },
                new Auction() { ID = 2, Title = "Auction B", Subtitle = "Auction B Subtitle", Description = "Auction B Description", MinimumPrice = 0M },
                new Auction() { ID = 3, Title = "Auction C", Subtitle = "Auction C Subtitle", Description = "Auction C Description", MinimumPrice = 1.00M },
                new Auction() { ID = 4, Title = "Auction D", Subtitle = "Auction D Subtitle", Description = "Auction D Description", MinimumPrice = 0M },
                new Auction() { ID = 5, Title = "Auction E", Subtitle = "Auction E Subtitle", Description = "Auction E Description", MinimumPrice = 150.00M },
                new Auction() { ID = 6, Title = "Auction F", Subtitle = "Auction F Subtitle", Description = "Auction F Description", MinimumPrice = 0M }
            };
            auctionRepo = new Mock<IRepo<Auction>>();

            // Entities calls should return the repo data:
            auctionRepo.Setup(repo => repo.Entities).Returns(auctionData);

            // Add calls should shove any new items into the list:
            auctionRepo.Setup(repo => repo.Add(It.IsAny<Auction>())).Callback<Auction>((auction) => auctionData.Add(auction));

            // Mock Find to look up the IDs
            auctionRepo.Setup(repo => repo.Find(It.IsAny<int>())).Returns<int>(id => auctionData.FirstOrDefault(auc => auc.ID == id));

            // Mock Delete
            auctionRepo.Setup(repo => repo.Delete(It.IsAny<Auction>())).Callback<Auction>(auction => auctionData.Remove(auction));
        }

        [TestMethod]
        public void Index()
        {
            AuctionController controller = new AuctionController(auctionRepo.Object);

            ViewResult result = controller.Index() as ViewResult;

            // check the result is usable, first
            Assert.IsNotNull(result);

            IEnumerable<Auction> viewModel = (IEnumerable<Auction>)result.Model;
            Auction[] auctions = viewModel.ToArray();

            // assert the model itself isn't null
            Assert.IsNotNull(auctions);

            // check the right number of products come back
            Assert.AreEqual(6, auctions.Length);
        }

        [TestMethod]
        public void Create()
        {
            Auction newAuction = new Auction() { ID = 7, Title = "New Auction", Subtitle = "New Auction Subtitle", Description = "I will put a description here" };

            AuctionController controller = new AuctionController(auctionRepo.Object);

            ActionResult result = controller.Create(newAuction);
            Auction[] auctionDataArray = auctionData.ToArray();

            // Verify we got a result before tinkering with it
            Assert.IsNotNull(result);

            ViewResult viewResult = result as ViewResult;

            // Assert the feedback flash message is correct:
            Assert.AreEqual(AuctionController.FlashMessageCreateSuccess, controller.TempData[controller.GetFlashKey(Models.FlashKeyType.Success)]);

            // Verify that `add` gets called
            auctionRepo.Verify(act => act.Add(newAuction), Times.Once);

            // Verify the object was added to the list
            Assert.IsNotNull(auctionDataArray);
            Assert.AreEqual(auctionDataArray.Length, 7);
        }

        [TestMethod]
        public void Edit()
        {
            // Made a mistake with #1, i'm actually selling user data, not "Auction 1" </failing_startup>
            const string ModifiedTitle = "Auctioneer User Data";

            Auction editedAuction = auctionData.First(auc => auc.ID == 1);
            AuctionController controller = new AuctionController(auctionRepo.Object);

            // Edit the auction and prep it to go back in
            editedAuction.Title = ModifiedTitle;

            ActionResult result = controller.Edit(editedAuction);
            Assert.IsNotNull(result);

            // Check the edit got called
            auctionRepo.Verify(act => act.Update(editedAuction), Times.Once);

            Assert.AreEqual(ModifiedTitle, auctionData.First(auc => auc.ID == 1).Title);
        }

        [TestMethod]
        public void Delete()
        {
            AuctionController controller = new AuctionController(auctionRepo.Object);

            ActionResult result = controller.Delete(1);
            Assert.IsNotNull(result);

            Auction[] auctionArray = auctionData.ToArray();

            // Assert it's been removed:
            Assert.IsNull(auctionArray.FirstOrDefault(auc => auc.ID == 1));
            Assert.AreEqual(5, auctionArray.Length);
        }

        [TestMethod]
        public void PlaceBid_Valid()
        {
            AuctionController controller = new AuctionController(auctionRepo.Object);

            ActionResult result = controller.Bid(1, new Bid() { Amount = 12.51M, Auction_ID = 1 });
            Assert.IsNotNull(result);

            PartialViewResult pvResult = result as PartialViewResult;

            // Assert the right view is returned:
            Assert.IsNotNull(pvResult);
            Assert.AreEqual("BidPlacePartial", pvResult.ViewName);

            // Assert the model is correct:
            PlaceBidViewModel viewModel = (PlaceBidViewModel)pvResult.Model;
            Assert.AreEqual(auctionData.FirstOrDefault(auc => auc.ID == 1), viewModel.Auction);

            Bid viewModelBid = viewModel.WinningBid;
            Assert.AreEqual(12.51M, viewModelBid.Amount);
        }

        [TestMethod]
        public void PlaceBid_ValidExtreme()
        {
            AuctionController controller = new AuctionController(auctionRepo.Object);

            // Place a bid for £12,000,00.51, high but valid:
            ActionResult result = controller.Bid(1, new Bid() { Amount = 12000000.51M, Auction_ID = 1 });
            Assert.IsNotNull(result);

            PartialViewResult pvResult = result as PartialViewResult;

            // Assert the right view is returned:
            Assert.IsNotNull(pvResult);
            Assert.AreEqual("BidPlacePartial", pvResult.ViewName);

            // Assert the model is correct:
            PlaceBidViewModel viewModel = (PlaceBidViewModel)pvResult.Model;
            Assert.AreEqual(auctionData.FirstOrDefault(auc => auc.ID == 1), viewModel.Auction);

            Bid viewModelBid = viewModel.WinningBid;
            Assert.AreEqual(12000000.51M, viewModelBid.Amount);
        }

        [TestMethod]
        public void PlaceBid_InvalidUnderReservePrice()
        {
            AuctionController controller = new AuctionController(auctionRepo.Object);

            // Place a bid for £1.21, under the Reserve Price of £12.51
            ActionResult result = controller.Bid(1, new Bid() { Amount = 1.51M, Auction_ID = 1 });
            Assert.IsNotNull(result);

            PartialViewResult pvResult = result as PartialViewResult;

            // Assert the right view is returned:
            Assert.IsNotNull(pvResult);
            Assert.AreEqual("BidPlacePartial", pvResult.ViewName);

            // Assert the model is correct:
            PlaceBidViewModel viewModel = (PlaceBidViewModel)pvResult.Model;
            Assert.AreEqual(auctionData.FirstOrDefault(auc => auc.ID == 1), viewModel.Auction);

            // Should not have altered the WinningBid for the Auction
            Bid viewModelBid = viewModel.WinningBid;
            Assert.AreEqual(12.50M, viewModelBid.Amount);
        }
    }
}