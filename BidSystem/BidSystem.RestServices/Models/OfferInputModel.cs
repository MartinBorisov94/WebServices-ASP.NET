namespace BidSystem.RestServices.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OfferInputModel
    {

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public double InitialPrice { get; set; }

        [Required]
        public DateTime ExpirationDateTime { get; set; }
    }
}