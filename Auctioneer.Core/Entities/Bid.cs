using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Core.Entities
{
    public class Bid
    {
        [Key]
        public int ID { get; set; }

        public virtual Auction Auction { get; set; }

        [Required]
        [Display(Name = "Bid Amount")]
        public decimal Amount { get; set; }
    }
}
