using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Auctioneer.Core.Entities;
using System.Collections.Generic;

namespace Auctioneer.Tests.Entities
{
    [TestClass]
    public class AuctionTest
    {
        private static Auction auction;
        private static List<Bid> bidData;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            auction = new Auction() { ID = 1, Description = "Test Description", Subtitle = "Test Subtitle", EndDate = DateTime.Now, MinimumPrice = 12.50M, Title = "Test Title", Bids = new List<Bid>() };

            bidData = new List<Bid>()
            {
                new Bid() { ID = 1, Amount = 10M, AuctioneerUser_Id = Guid.NewGuid().ToString() },
                new Bid() { ID = 2, Amount = 20M, AuctioneerUser_Id = Guid.NewGuid().ToString() },
                new Bid() { ID = 3, Amount = 30M, AuctioneerUser_Id = Guid.NewGuid().ToString() },
                new Bid() { ID = 4, Amount = 40M, AuctioneerUser_Id = Guid.NewGuid().ToString() },
            };
        }

        [TestMethod]
        public void WinningBid_Single()
        {
            Bid addedBid = bidData.Find(b => b.ID == 1);
            auction.Bids.Add(addedBid);

            Bid winningBid = auction.WinningBid();
            Assert.IsNotNull(winningBid);

            Assert.AreEqual(addedBid.Amount, winningBid.Amount);
        }

        [TestMethod]
        public void WinningBid_Multiple()
        {
            foreach (Bid bid in bidData.FindAll(b => b.ID < 3))
            {
                auction.Bids.Add(bid);
            }

            Bid expectedBid = bidData.Find(b => b.ID == 2);
            Assert.IsNotNull(expectedBid);

            Bid winningBid = auction.WinningBid();
            Assert.IsNotNull(winningBid);

            Assert.AreEqual(expectedBid.Amount, winningBid.Amount);
        }

        [TestMethod]
        /**
         * For if the bids somehow come through incorrectly, the Winning Bid
         * should always be the highest bid placed so far.
         */
        public void WinningBid_Incorrectly_Ordered()
        {
            List<Bid> bids = bidData.FindAll(b => b.ID <= 4);
            bids.Reverse();

            foreach (Bid bid in bids)
            {
                auction.Bids.Add(bid);
            }

            Bid expectedBid = bidData.Find(b => b.ID == 4);
            Assert.IsNotNull(expectedBid);

            Bid winningBid = auction.WinningBid();
            Assert.IsNotNull(winningBid);

            Assert.AreEqual(expectedBid.Amount, winningBid.Amount);
        }
    }
}
