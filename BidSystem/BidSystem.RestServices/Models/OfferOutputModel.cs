namespace BidSystem.RestServices.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Owin.Security.DataHandler.Serializer;

    public class OfferOutputModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Seller { get; set; }

        public DateTime DatePublished { get; set; }

        public double InitialPrice { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public bool IsExpired { get; set; }

        public string BidWinner { get; set; }

        public virtual IEnumerable<BidsOutputModel> Bids { get; set; } 


    }
}