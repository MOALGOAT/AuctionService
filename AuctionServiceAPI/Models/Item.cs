using MongoDB.Bson.Serialization.Attributes;

namespace AuctionServiceAPI.Models
{
    public class Item
    {
        [BsonId]
        Guid _id { get; set; }
     public string title { get; set; }
    public string description { get; set; }
    public string category { get; set; }
        float startingPrice { get; set; }
        string status { get; set; }

    }
}
