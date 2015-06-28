namespace BidSystem.RestServices.Models
{
    using System;

    public class BidsOutputModel
    {
        public int Id { get; set; }

        public double OfferedPrice { get; set; }

        public string Bidder { get; set; }

        public DateTime DateCreated { get; set; }

        public string Comment { get; set; }

        public int OfferId { get; set; }

    }
}
