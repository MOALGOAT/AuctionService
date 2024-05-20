﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuctionServiceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
        public async Task<ActionResult<int>> AddAuction(Auction auction)
        {
            _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

            var auctionId = await _auctionService.AddAuction(auction);
            return Ok(auctionId);
        }

        [HttpPut("{_id}")]
        public async Task<IActionResult> UpdateAuction(Guid _id, Auction auction)
        {
            _logger.LogInformation(1, $"XYZ Service responding from {GetIpAddress()}");

            if (_id != auction._id)
            {
                return BadRequest();
            }

            var result = await _auctionService.UpdateAuction(auction);
            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{_id}")]
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
    }
}
