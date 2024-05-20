﻿using MongoDB.Bson.Serialization.Attributes;

namespace AuctionServiceAPI.Models
{
    public class Bid
    {
        [BsonId]
        Guid _id { get; set; }
        User user { get; set; }
        float bidPrice { get; set; }
    }
}