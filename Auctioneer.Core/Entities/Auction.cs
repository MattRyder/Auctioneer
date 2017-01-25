using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Core.Entities
{
    public class Auction
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Subtitle { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        
        public decimal MinimumPrice { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        public Bid WinningBid()
        {
            if (Bids == null) return new Bid();
            return Bids.OrderBy(b => b.Amount).LastOrDefault();
        }
    }
}
