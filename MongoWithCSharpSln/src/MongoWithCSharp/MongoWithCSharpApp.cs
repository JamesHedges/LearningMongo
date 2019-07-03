using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoWithCSharp
{
    public class MongoWithCSharpApp
    {
        public async Task RunAsync()
        {
            var client = new MongoClient(@"mongodb://localhost:27017");
            var dbs = client.ListDatabasesAsync();

            await ShowDbs(client);

            var database = client.GetDatabase("Test");
            Console.WriteLine($"Connected to Database: {database.DatabaseNamespace.DatabaseName}");

            await ShowCollections(database);

            var collection = database.GetCollection<BsonDocument>("Collection1");
            await ShowRecords(collection);
        }

        private async Task ShowDbs(IMongoClient client)
        {
            var cursor = await client.ListDatabaseNamesAsync();
            await cursor.ForEachAsync(db => Console.WriteLine($"DB: {db}"));
        }

        private async Task ShowCollections(IMongoDatabase db)
        {
                var collectionsCursor = await db.ListCollectionNamesAsync();
                await collectionsCursor.ForEachAsync(col => Console.WriteLine($"Collection: {col.ToString()}"));
        }

        private async Task ShowRecords(IMongoCollection<BsonDocument> collection)
        {
            using (var results = await collection.FindAsync(new BsonDocument()))
            {
                while (await results.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = results.Current;
                    foreach(BsonDocument document in batch)
                    {
                        Console.WriteLine(document);
                    }
                }
            }
        }
    }
}