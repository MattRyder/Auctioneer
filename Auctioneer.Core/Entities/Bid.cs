using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctioneer.Core.Entities
{
    public class Bid
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Auction")]
        public virtual int Auction_ID { get; set; }

        public virtual Auction Auction { get; set; }

        [Required]
        [Display(Name = "Bid Amount")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#}")]
        public decimal Amount { get; set; }
    }
}
