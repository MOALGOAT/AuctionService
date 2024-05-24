﻿using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionServiceAPI.Models;
using AuctionServiceAPI.Service;

namespace AuctionServiceAPI.Models
{
    public interface IAuctionService
    {
        Task<Auction?> GetAuction(Guid auctionId);
        Task<IEnumerable<Auction>?> GetAuctionList();
        Task<Guid> AddAuction(Auction auction);
        Task<long> UpdateAuction(Auction auction);
        Task<long> DeleteAuction(Guid auctionId);
        Task ProcessMessageAsync(string message);
        Task ProcessBidAsync (Bid bid);
    }

    public class AuctionMongoDBService : IAuctionService
    {
        private readonly ILogger<AuctionMongoDBService> _logger;
        private readonly IMongoCollection<Auction> _auctionCollection;
        public async Task ProcessMessageAsync(string message)
            {
                // Implement your message processing logic here
                _logger.LogInformation($"Processing message: {message}");
                // Example: Parse the message and perform operations
            }


        public async Task ProcessBidAsync(Bid bid)
        {
            var filter = Builders<Auction>.Filter.Eq(a => a._id, bid.auctionId);
            var update = Builders<Auction>.Update.Push(a => a.bids, bid);
            await _auctionCollection.UpdateOneAsync(filter, update);
            _logger.LogInformation($"Bid processed for auction {bid.auctionId}");
        }

        public AuctionMongoDBService(ILogger<AuctionMongoDBService> logger, MongoDBContext dbContext, IConfiguration configuration)
        {
            var collectionName = configuration["collectionName"];
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ApplicationException("AuctionCollectionName is not configured.");
            }

            _logger = logger;
            _auctionCollection = dbContext.GetCollection<Auction>(collectionName);
            _logger.LogInformation($"Collection name: {collectionName}");
        }

        public async Task<Auction?> GetAuction(Guid auctionId)
        {
            var filter = Builders<Auction>.Filter.Eq(x => x._id, auctionId);
            return await _auctionCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Auction>?> GetAuctionList()
        {
            return await _auctionCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Guid> AddAuction(Auction auction)
        {
            await _auctionCollection.InsertOneAsync(auction);
            return auction._id;
        }

        public async Task<long> UpdateAuction(Auction auction)
        {
            var filter = Builders<Auction>.Filter.Eq(x => x._id, auction._id);
            var result = await _auctionCollection.ReplaceOneAsync(filter, auction);
            return result.ModifiedCount;
        }

        public async Task<long> DeleteAuction(Guid auctionId)
        {
            var filter = Builders<Auction>.Filter.Eq(x => x._id, auctionId);
            var result = await _auctionCollection.DeleteOneAsync(filter);
            return result.DeletedCount;
        }
    }
}
