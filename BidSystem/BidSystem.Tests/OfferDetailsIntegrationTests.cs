namespace BidSystem.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class OfferDetailsIntegrationTests
    {
        [TestMethod]
        public void GetDetailsNonExistingOffer_ShouldReturn404NotFound()
        {
            // Arrange -> clean the DB
            TestingEngine.CleanDatabase();

            // Act -> delete the channel
            var httpDeleteResponse = TestingEngine.HttpClient.GetAsync("/api/offers/details/1").Result;

            // Assert -> HTTP status code is 404 (Not Found)
            Assert.AreEqual(HttpStatusCode.NotFound, httpDeleteResponse.StatusCode);
        }
    }
}
