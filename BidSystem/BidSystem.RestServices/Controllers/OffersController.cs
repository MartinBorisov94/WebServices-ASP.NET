namespace BidSystem.RestServices.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using System.Web.Http.Description;

    using BidSystem.Data;
    using BidSystem.Data.Models;
    using BidSystem.RestServices.Models;

    using Microsoft.AspNet.Identity;

    public class OffersController : ApiController
    {
        private BidSystemDbContext db = new BidSystemDbContext();

        // GET: api/offers/all
        [Route("api/offers/all")]
        public IHttpActionResult GetOffers()
        {
            var offers = this.db.Offers
                .OrderBy(o => o.PublishDate)
                .ThenBy(o => o.Id)
                .Select(o => new OffersOutputModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = o.ExpirationDate < DateTime.Now ? true : false,
                    BidsCount = o.Bids.Count,
                    BidWinner = o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName ?? null
                });
            return this.Ok(offers);
        }


        // GET: api/offers/active
        [Route("api/offers/active")]
        public IHttpActionResult GetActive()
        {
            var offers = this.db.Offers
                .Where(o => o.ExpirationDate > DateTime.Now)
                .OrderBy(o => o.ExpirationDate)
                .ThenBy(o => o.Id)
                .Select(o => new OffersOutputModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = o.ExpirationDate < DateTime.Now ? true : false,
                    BidsCount = o.Bids.Count,
                    BidWinner = o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName ?? null
                });
            return this.Ok(offers);
        }

        // GET: api/offers/expired
        [Route("api/offers/expired")]
        public IHttpActionResult GetExpired()
        {
            var offers = this.db.Offers
                .Where(o => o.ExpirationDate < DateTime.Now)
                .OrderBy(o => o.ExpirationDate)
                .ThenBy(o => o.Id)
                .Select(o => new OffersOutputModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = o.ExpirationDate < DateTime.Now ? true : false,
                    BidsCount = o.Bids.Count,
                    BidWinner = o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName ?? null
                });
            return this.Ok(offers);
        }


        // GET: api/offers/details/{id}
        [ResponseType(typeof(Offer))]
        [Route("api/offers/details/{id}")]
        public IHttpActionResult GetOfferById(int id)
        {

            var offer = this.db.Offers
                .Select(o => new OfferOutputModel()
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = o.ExpirationDate < DateTime.Now ? true : false,
                    BidWinner = o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName ?? null,
                    Bids = o.Bids
                        .OrderByDescending(b => b.Date)
                        .ThenByDescending(b => b.Id)
                        .Select(b => new BidsOutputModel()
                                                  {
                                                      Id = b.Id,
                                                      OfferId = b.OfferId,
                                                      DateCreated = b.Date,
                                                      Bidder = b.Bidder.UserName,
                                                      OfferedPrice = b.BidPrice,
                                                      Comment = b.Comment
                                                  })
                })
                .FirstOrDefault(o => o.Id == id);

            if (offer == null)
            {
                return this.NotFound();
            }

            return this.Ok(offer);
        }

        // GET: api/offers/active
        [Route("api/offers/my")]
        public IHttpActionResult GetMy()
        {
            var currentUserId = this.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return this.Unauthorized();
            }

            var offers = this.db.Offers
                .Where(o => o.SellerId == currentUserId)
                .OrderBy(o => o.PublishDate)
                .ThenBy(o => o.Id)
                .Select(o => new OffersOutputModel
                {
                    Id = o.Id,
                    Title = o.Title,
                    Description = o.Description,
                    Seller = o.Seller.UserName,
                    DatePublished = o.PublishDate,
                    InitialPrice = o.InitialPrice,
                    ExpirationDateTime = o.ExpirationDate,
                    IsExpired = o.ExpirationDate < DateTime.Now ? true : false,
                    BidsCount = o.Bids.Count,
                    BidWinner = o.Bids.OrderByDescending(b => b.BidPrice).FirstOrDefault().Bidder.UserName ?? null
                });

            return this.Ok(offers);
        }

        // POST: pi/offers
        [HttpPost]
        [ResponseType(typeof(Offer))]
        [Route("api/offers", Name = "PostOffer")]
        public IHttpActionResult PostOffer(OfferInputModel offerData)
        {
            if (!ModelState.IsValid || offerData == null)
            {
                return this.BadRequest(ModelState);
            }

            var currentUserId = this.User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return this.Unauthorized();
            }


            var offer = new Offer()
            {
                Title = offerData.Title,
                Description = offerData.Description,
                InitialPrice = offerData.InitialPrice,
                ExpirationDate = offerData.ExpirationDateTime,
                PublishDate = DateTime.Now,
                SellerId = currentUserId
            };

            this.db.Offers.Add(offer);
            this.db.SaveChanges();

            var currentUserName = User.Identity.GetUserName();
            return this.CreatedAtRoute(
                "PostOffer",
                new { id = offer.Id },
                new { id = offer.Id, Seller = currentUserName, Message = "Offer created." });
        }

        // POST: api/offers/{id}/bid
        [HttpPost]
        [ResponseType(typeof(Bid))]
        [Route("api/offers/{id}/bid")]
        public IHttpActionResult PostBidById(int id, BidInputModel bidData)
        {
            if (!ModelState.IsValid || bidData == null)
            {
                return this.BadRequest(ModelState);
            }

            var bid = this.db.Offers.Find(id);
            if (bid == null)
            {
                return this.NotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return this.Unauthorized();
            }

            var offerDate = this.db.Offers.Where(o => o.Id == id).Select(o => o.ExpirationDate).First();

            if (offerDate < DateTime.Now)
            {
                return this.BadRequest("Offer has expired.");
            }

            var bestBidPrice = this.db.Bids
                .OrderByDescending(b => b.Id)
                .Where(b => b.OfferId == id)
                .Select(b => b.BidPrice)
                .FirstOrDefault();


            if (bestBidPrice >= bidData.BidPrice)
            {
                return this.BadRequest("Your bid should be > " + bestBidPrice);
            }

            var bit = new Bid()
                          {
                              BidderId = currentUserId,
                              BidPrice = bidData.BidPrice,
                              Comment = bidData.Comment,
                              Date = DateTime.Now,
                              OfferId = id
                          };

            this.db.Bids.Add(bit);
            this.db.SaveChanges();

            return this.Ok(new
            {
                Id = bit.Id,
                Author = User.Identity.GetUserName(),
                Message = "Bid created."
            });


        }
    }
}
