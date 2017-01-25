using Auctioneer.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Auctioneer.Models
{
    public class AuctionDetailViewModel
    {
        public Auction Auction { get; set; }
        public Bid Bid { get; set; }
    }

    public class PlaceBidViewModel
    {
        public Auction Auction { get; set; }
        public Bid Bid { get; set; }
        public Bid WinningBid { get; set; }
    }
}