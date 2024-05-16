using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AuctionServiceAPI.Tests
{
    [TestFixture]
    public class AuctionModelTests
    {
        [Test]
        public void AuctionModel_Properties_Success()
        {
            // Arrange
            var auction = new Auction
            {
                _id = 1,
                Item = new Item(),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Bids = new List<Bid>()
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                Assert.That(auction._id, Is.EqualTo(1));
                Assert.IsNotNull(auction.Item);
                Assert.IsNotNull(auction.StartTime);
                Assert.IsNotNull(auction.EndTime);
                Assert.IsNotNull(auction.Bids);
            });
        }

        [Test]
        public void AuctionModel_InvalidEndTime_Failure()
        {
            // Arrange
            var auction = new Auction
            {
                _id = 1,
                Item = new Item(),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(-1), // Setting end time in the past
                Bids = new List<Bid>()
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                // Attempting to create auction with end time in the past should throw ArgumentException
                _ = auction;
            });
        }
    }
}
