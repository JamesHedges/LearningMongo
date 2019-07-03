using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoWithCSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var client = new MongoClient(@"mongodb://host:27017");
            var dbs = client.ListDatabasesAsync();

            //await ShowDbs(client);

            var database = client.GetDatabase("Test");
            Console.WriteLine($"Connected to Database: {database.DatabaseNamespace.DatabaseName}");

            var collection = database.GetCollection<BsonDocument>("Collection1");

            // var collections =  database.ListCollectionsAsync().Result.ToListAsync<BsonDocument>().Result;
            // foreach(var collection in collections)
            // {
            //     Console.WriteLine($"Collection: {collection.ToString()}");
            // }
        }

        static async Task ShowDbs(IMongoClient client)
        {
            var cursor = await client.ListDatabaseNamesAsync();
            await cursor.ForEachAsync(db => Console.WriteLine($"DB: {db}"));
        }
    }
}
