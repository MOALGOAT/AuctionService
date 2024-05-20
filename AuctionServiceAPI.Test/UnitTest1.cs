using NUnit.Framework;
using System;
using System.Collections.Generic;
using AuctionServiceAPI.Models;

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
                _id = Guid.NewGuid(),
                item = new Item(),
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddHours(1),
                bids = new List<Bid>()
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                Assert.IsInstanceOf<Item>(auction.item);
                Assert.IsInstanceOf<DateTime>(auction.startTime);
                Assert.IsInstanceOf<DateTime>(auction.endTime);
                Assert.IsInstanceOf<List<Bid>>(auction.bids);
            });
        }

        [Test]
        public void AuctionModel_InvalidEndTime_Failure()
        {
            // Arrange
            var auction = new Auction
            {
                _id = Guid.NewGuid(),
                item = new Item(),
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddHours(-1), // Setting end time in the past
                bids = new List<Bid>()
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
                _id = Guid.NewGuid(),
                item = new Item(),
                startTime = DateTime.Now,
                endTime = DateTime.Now.AddHours(1),
                bids = bids
            };

            // Act & Assert
            Assert.That(auction.bids, Is.Not.Null);
            Assert.AreEqual(2, auction.bids.Count);
            Assert.Contains(bid1, auction.bids);
            Assert.Contains(bid2, auction.bids);
        }

        [Test]
        public void AuctionModel_NullItem_Failure()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                var auction = new Auction
                {
                    _id = Guid.NewGuid(),
                    item = null, // Setting item to null should throw ArgumentNullException
                    startTime = DateTime.Now,
                    endTime = DateTime.Now.AddHours(1),
                    bids = new List<Bid>()
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
                    _id = Guid.NewGuid(),
                    item = new Item(),
                    startTime = DateTime.Now,
                    endTime = DateTime.Now.AddHours(1),
                    bids = bids
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
                    _id = Guid.NewGuid(),
                    item = new Item(),
                    startTime = DateTime.Now.AddHours(1), // Setting start time after end time
                    endTime = DateTime.Now,
                    bids = new List<Bid>()
                };
            });
        }
    }
}
