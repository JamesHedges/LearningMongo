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
            var context = TestMongoContext.Create(@"mongodb://localhost:27017");
            await context.ClearAsync("People");
            var repo = new TestRepository(context);

            var entities = CreateTestEntities();
            await repo.InsertManyAsync(entities);

            await ShowRecords(repo);

            var person = await repo.FindPersonByFirstName("Jim");
            Console.WriteLine($"\nFound: {person.FirstName} {person.LastName}");
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

        private async Task ShowRecords(TestRepository repo)
        {
            var people = await repo.GetPeople();
            foreach (var person in people)
            {
                Console.WriteLine($"Id: {person.Id}, FName: {person.FirstName}, LName: {person.LastName}, Age: {person.Age}");
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