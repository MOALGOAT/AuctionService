using MongoDB.Bson.Serialization.Attributes;

namespace AuctionServiceAPI.Models
{
    public class Item
    {
        [BsonId]
        Guid _id { get; set; }
        string title { get; set; }
        string description { get; set; }
        string category { get; set; }
        float startingPrice { get; set; }
        string status { get; set; }

    }
}
