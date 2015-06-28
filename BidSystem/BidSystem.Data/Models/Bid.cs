namespace BidSystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Bid
    {
        [Key]
        public int Id { get; set; }

        public double BidPrice { get; set; }

        public string BidderId { get; set; }

        public virtual User Bidder { get; set; }

        public DateTime Date { get; set; }

        public string Comment { get; set; }

        public int OfferId { get; set; }
    }
}
