using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoWithCSharp.Dal;

namespace MongoWithCSharp
{
    public class MongoWithCSharpApp
    {
        public async Task RunAsync()
        {
            var context = new DataContext<PersonEntity>(@"mongodb://localhost:27017", "Test", "People");
            await context.ClearAsync();
            var entities = CreateTestEntities();
            await context.InsertManyAsync(entities);

            await ShowRecords(context.GetCollection());
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

        private async Task ShowRecords(IMongoCollection<PersonEntity> collection)
        {
            using (var results = await collection.FindAsync(new BsonDocument()))
            {
                while (await results.MoveNextAsync())
                {
                    IEnumerable<PersonEntity> batch = results.Current;
                    foreach(PersonEntity document in batch)
                    {
                        Console.WriteLine($"Id: {document.Id}, FName: {document.FirstName}, LName: {document.LastName}, Age: {document.Age}");
                    }
                }
            }
        }

        private IEnumerable<PersonEntity> CreateTestEntities()
        {
            return new List<PersonEntity>
            {
                new PersonEntity{FirstName="Anna", LastName="Hedges", Age=56},
                new PersonEntity{FirstName="Jim", LastName="Hedges", Age=56},
                new PersonEntity{FirstName="Joe", LastName="Tester", Age=44},
                new PersonEntity{FirstName="Mary", LastName="Contrary", Age=18}
            };
        }
    }
}