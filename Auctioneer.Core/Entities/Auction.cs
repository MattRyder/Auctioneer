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
        [Display(Name = "Auction Name", Description = "The name of what you are auctioning")]
        public string Title { get; set; }

        [MaxLength(140, ErrorMessage = "Subtitle must be less than 140 characters")]
        [Display(Name = "Auction Subtitle", Description = "A brief explanation of the auction item's features, max. 140 characters")]
        public string Subtitle { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        
        [DataType(DataType.Currency)]
        [Display(Name = "Reserve Price", Description = "The minimum bid you will accept, optional")]
        public decimal MinimumPrice { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }

        public Bid WinningBid()
        {
            Bid winningBid = (Bids != null) ? Bids.OrderBy(b => b.Amount).LastOrDefault() : null;
            return (winningBid != null) ? winningBid : new Bid() { Amount = MinimumPrice };
        }
    }
}
