using MongoDB.Bson.Serialization.Attributes;

namespace MongoWithCSharp.Dal
{
    public class PersonEntity
    {
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}