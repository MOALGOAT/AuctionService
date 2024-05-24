using MongoDB.Bson.Serialization.Attributes;

namespace AuctionServiceAPI.Models
{
    public class User
    {
        [BsonId]
        public Guid _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string adress { get; set; }
        public string telephonenumber { get; set; }
        public string role { get; set; }    
        public string email { get; set; }
        public DateTime created_at { get; set; }

    }
}
