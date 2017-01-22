using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Auctioneer.Core.Abstract;
using Auctioneer.Core.Entities;
using Moq;
using System.Collections.Generic;
using Auctioneer.Controllers;
using System.Web.Mvc;
using System.Linq;

namespace Auctioneer.Tests.Controllers
{
    [TestClass]
    public class AuctionControllerTest
    {
        private static Mock<IRepo<Auction>> auctionRepo;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            IEnumerable<Auction> auctionData = new List<Auction>()
            {
                new Auction() { ID = 1, Title = "Auction A", Subtitle = "Auction A Subtitle", Description = "Auction A Description" },
                new Auction() { ID = 2, Title = "Auction B", Subtitle = "Auction B Subtitle", Description = "Auction B Description" },
                new Auction() { ID = 3, Title = "Auction C", Subtitle = "Auction C Subtitle", Description = "Auction C Description" },
                new Auction() { ID = 4, Title = "Auction D", Subtitle = "Auction D Subtitle", Description = "Auction D Description" },
                new Auction() { ID = 5, Title = "Auction E", Subtitle = "Auction E Subtitle", Description = "Auction E Description" },
                new Auction() { ID = 6, Title = "Auction F", Subtitle = "Auction F Subtitle", Description = "Auction F Description" }
            };
            auctionRepo = new Mock<IRepo<Auction>>();
            auctionRepo.Setup(repo => repo.Entities).Returns(auctionData);
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
    }
}
