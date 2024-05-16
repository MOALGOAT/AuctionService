using Microsoft.Extensions.Logging;
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
        Task<Auction?> GetAuction(int auctionId);
        Task<IEnumerable<Auction>?> GetAuctionList();
        Task<int> AddAuction(Auction auction);
        Task<long> UpdateAuction(Auction auction);
        Task<long> DeleteAuction(int auctionId);
    }

    public class AuctionMongoDBService : IAuctionService
    {
        private readonly ILogger<AuctionMongoDBService> _logger;
        private readonly IMongoCollection<Auction> _auctionCollection;

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

        public async Task<Auction?> GetAuction(int auctionId)
        {
            var filter = Builders<Auction>.Filter.Eq(x => x._id, auctionId);
            return await _auctionCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Auction>?> GetAuctionList()
        {
            return await _auctionCollection.Find(_ => true).ToListAsync();
        }

        public async Task<int> AddAuction(Auction auction)
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

        public async Task<long> DeleteAuction(int auctionId)
        {
            var filter = Builders<Auction>.Filter.Eq(x => x._id, auctionId);
            var result = await _auctionCollection.DeleteOneAsync(filter);
            return result.DeletedCount;
        }
    }
}
