using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuctionServiceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuctionServiceAPI.Service;
using Microsoft.AspNetCore.Authorization;


namespace AuctionServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(IAuctionService auctionService, ILogger<AuctionController> logger)
        {
            _auctionService = auctionService;
            _logger = logger;
        }

        private string GetIpAddress()
        {
            var hostName = Dns.GetHostName();
            var ips = Dns.GetHostAddresses(hostName);
            var ipaddr = ips.First().MapToIPv4().ToString();
            return ipaddr;
        }

        [HttpGet("{_id}")]
        [Authorize(Roles = "1,2")]
        public async Task<ActionResult<Auction>> GetAuction(Guid _id)
        {
            _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

            var auction = await _auctionService.GetAuction(_id);
            if (auction == null)
            {
                return NotFound();
            }
            return auction;
        }

        [HttpGet]
        [Authorize(Roles = "1,2")]
        public async Task<ActionResult<IEnumerable<Auction>>> GetAuctionList()
        {
            _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

            var auctionList = await _auctionService.GetAuctionList();

            if (auctionList == null)
            {
                throw new ApplicationException("Auction list is null");
            }
            return Ok(auctionList);
        }

        [HttpPost]
        [Authorize(Roles = "2")]
        public async Task<ActionResult<int>> AddAuction(Auction auction)
        {
            try
            {
                _logger.LogInformation($"Received request to add auction: {auction._id}");
                _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

                var auctionId = await _auctionService.AddAuction(auction);
                return Ok(auctionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while adding an auction.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPut("{_id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> UpdateAuction(Guid _id, Auction auction)
        {
            _logger.LogInformation(1, $"auction Service responding from {GetIpAddress()}");

            if (_id != auction._id)
            {
                return BadRequest("ding ding ding ding du fik et badrequest fordi id ikke matcher! :D");
            }

            var result = await _auctionService.UpdateAuction(auction);
            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{_id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> DeleteAuction(Guid _id)
        {
            _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

            var result = await _auctionService.DeleteAuction(_id);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("/api/legal/auctions/{auctionId}")]
        [Authorize(Roles = "3")]
        public async Task<ActionResult<object>> GetLegalAuction(Guid auctionId)
        {
            _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

            var auction = await _auctionService.GetAuction(auctionId);
            if (auction == null)
            {
                return NotFound(new { error = "Auction not found" });
            }

            var response = new
            {
                id = auction._id,
                title = auction.item.title,
                description = auction.item.description,
                startDate = auction.startTime,
                endDate = auction.endTime,
                currentBid = auction.bids.Any() ? auction.bids.Max(b => b.bidPrice) : 0,
                createdBy = auction.seller.username, // Assuming CreatedBy can be the seller's username
                createdAt = auction.startTime // Assuming CreatedAt is the same as startTime
            };

            return Ok(response);
        }

        [HttpGet("/api/legal/auctions")]
        [Authorize(Roles = "3")]
        public async Task<ActionResult<IEnumerable<object>>> GetLegalAuctions()
        {
            _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

            var currentDate = DateTime.UtcNow.Date;

            var auctions = await _auctionService.GetAuctionList();

            auctions = auctions.Where(a => a.startTime.Date >= currentDate).ToList();

            if (!auctions.Any())
            {
                return NotFound(new { error = "No auctions found" });
            }

            var response = auctions.Select(a => new
            {
                id = a._id,
                title = a.item.title,
                description = a.item.description,
                startDate = a.startTime,
                endDate = a.endTime,
                currentBid = a.bids.Any() ? a.bids.Max(b => b.bidPrice) : 0,
                createdBy = a.seller.username,
                createdAt = a.startTime
            });

            return Ok(response);
        }

    }
}
