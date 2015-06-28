namespace BidSystem.RestServices.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using BidSystem.Data;
    using BidSystem.Data.Models;
    using BidSystem.RestServices.Models;

    using Microsoft.AspNet.Identity;

    public class BidsController : ApiController
    {
        private BidSystemDbContext db = new BidSystemDbContext();

        // GET: api/bids/my
        [Route("api/bids/my")]
        public IHttpActionResult GetMy()
        {
            var currentUserId = this.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return this.Unauthorized();
            }

            var bids = this.db.Bids
                .Where(b => b.BidderId == currentUserId)
                .OrderBy(b => b.Date)
                .ThenBy(b => b.Id)
                .Select(b => new BidsOutputModel
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    DateCreated = b.Date,
                    Bidder = b.Bidder.UserName,
                    OfferedPrice = b.BidPrice,
                    Comment = b.Comment
                });

            return this.Ok(bids);
        }

        // GET: api/bids/won
        [Route("api/bids/won")]
        public IHttpActionResult GetWon()
        {
            var currentUserId = this.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return this.Unauthorized();
            }

            //var bids = this.db.Bids
            //    .Where(b => b.Date < DateTime.Now && b.BidderId == currentUserId)
            //    .OrderByDescending(b => b.BidPrice)
            //    .Select(b => new BidsOutputModel
            //    {
            //        Id = b.Id,
            //        OfferId = b.OfferId,
            //        DateCreated = b.Date,
            //        Bidder = b.Bidder.UserName,
            //        OfferedPrice = b.BidPrice,
            //        Comment = b.Comment
            //    });




            var bids = this.db.Bids
                .OrderByDescending(b => b.BidPrice)
                .Where(b => b.Date < DateTime.Now && b.BidderId == currentUserId)
                .Select(b => new BidsOutputModel
                {
                    Id = b.Id,
                    OfferId = b.OfferId,
                    DateCreated = b.Date,
                    Bidder = b.Bidder.UserName,
                    OfferedPrice = b.BidPrice,
                    Comment = b.Comment
                })
                .First();

            return this.Ok(bids);
        }
    }
}
