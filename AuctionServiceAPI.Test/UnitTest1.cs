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

        [Test]
        public void AuctionModel_Bids_Success()
        {
            // Arrange
            var bid1 = new Bid();
            var bid2 = new Bid();
            var bids = new List<Bid> { bid1, bid2 };

            var auction = new Auction
            {
                _id = 1,
                Item = new Item(),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                Bids = bids
            };

            // Act & Assert
            Assert.That(auction.Bids, Is.Not.Null);
            Assert.AreEqual(2, auction.Bids.Count);
            Assert.Contains(bid1, auction.Bids);
            Assert.Contains(bid2, auction.Bids);
        }

        [Test]
        public void AuctionModel_NullItem_Failure()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                var auction = new Auction
                {
                    _id = 1,
                    Item = null, // Setting item to null should throw ArgumentNullException
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Bids = new List<Bid>()
                };
            });
        }

        [Test]
        public void AuctionModel_DuplicateBids_Failure()
        {
            // Arrange
            var bid1 = new Bid();
            var bids = new List<Bid> { bid1, bid1 }; // Adding same bid twice

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var auction = new Auction
                {
                    _id = 1,
                    Item = new Item(),
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Bids = bids
                };
            });
        }

        [Test]
        public void AuctionModel_StartTimeAfterEndTime_Failure()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var auction = new Auction
                {
                    _id = 1,
                    Item = new Item(),
                    StartTime = DateTime.Now.AddHours(1), // Setting start time after end time
                    EndTime = DateTime.Now,
                    Bids = new List<Bid>()
                };
            });
        }
    }
}
