using AuctionServiceAPI.Controllers;
using AuctionServiceAPI.Models;
using AuctionServiceAPI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AuctionServiceAPI.Tests
{
    [TestFixture]
    public class AuctionControllerTests
    {
        public Mock<IAuctionService> _mockAuctionService;
        public Mock<ILogger<AuctionController>> _mockLogger;
        public AuctionController _controller;

        [SetUp]
        public void Setup()  // SetUp er en metode der bliver kørt før hver test, så mock objekter og controller bliver initialiseret 
        {
            _mockAuctionService = new Mock<IAuctionService>();
            _mockLogger = new Mock<ILogger<AuctionController>>();
            _controller = new AuctionController(_mockAuctionService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAuction_ReturnsNotFound_WhenAuctionDoesNotExist() // Vi opsætter mock'en til at returnere null, når GetAuction metoden kaldes med en given auctionId.
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            _mockAuctionService.Setup(service => service.GetAuction(auctionId)).ReturnsAsync((Auction)null);

            // Act
            var result = await _controller.GetAuction(auctionId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetAuction_ReturnsAuction_WhenAuctionExists() // Vi opsætter mock'en til at returnere en Auction objekt, når GetAuction metoden kaldes med en given auctionId.
        {
            // Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction { _id = auctionId };
            _mockAuctionService.Setup(service => service.GetAuction(auctionId)).ReturnsAsync(auction);

            // Act
            var result = await _controller.GetAuction(auctionId);

            // Assert
            Assert.IsInstanceOf<ActionResult<Auction>>(result);
            Assert.IsInstanceOf<Auction>(result.Value);
            Assert.AreEqual(auctionId, result.Value._id);
        }
    }
}