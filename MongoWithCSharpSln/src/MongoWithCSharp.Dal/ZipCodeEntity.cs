using MongoDB.Bson.Serialization.Attributes;

namespace MongoWithCSharp.Dal
{
    public class ZipCodeEntity
    {
        [BsonId]
        [BsonElement(elementName: "_id")]
        public string Code { get; set; }
        [BsonElement(elementName: "city")]
        public string City { get; set; }
        //[BsonRepresentation(MongoDB.Bson.BsonType.Double)]
        [BsonElement(elementName: "loc")]
        public double[] Loc { get; set; }
        [BsonElement(elementName: "pop")]
        public int Pop { get; set; }
        [BsonElement(elementName: "state")]
        public string State { get; set; }
    }
}