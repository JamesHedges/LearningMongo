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
            var repo = new PeopleRepository(context);

            var entities = CreateTestEntities();
            await repo.InsertManyAsync(entities);

            await ShowRecords(repo);

            var person = await repo.FindPersonByFirstNameWithLinq("Jim");
            Console.WriteLine($"\nFound: {person.FirstName} {person.LastName}");

            var person2 = await repo.FindPersonByFirstAndLastName("Anna", "Hedges");
            Console.WriteLine($"\nFound: {person2.FirstName} {person2.LastName}");

            await TestReplacingOne(context);
            await TestReplacingOneAsUpsert(context);

            //var zipRepo = new ZipCodeRepository(context);
            //await LoadZipCodeData(zipRepo);
            //var zips = await zipRepo.LookupCityState("peoria", "il");
            //var zips = await zipRepo.LookupCity("dallas");
            //var zips = await zipRepo.LookupCityLinq("dallas");
            //var zips = await zipRepo.LookupZip("87107");
            //ShowZips(zips);
        }

        private async Task LoadZipCodeData(ZipCodeRepository repo)
        {
            Console.WriteLine($"Start Importing Zip Codes: {DateTime.Now}");
            await repo.ImportJsonAsync(@"C:\Data\TestData\zips.json");
            Console.WriteLine($"Finished Importing Zip Codes: {DateTime.Now}");
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

        private async Task ShowRecords(PeopleRepository repo)
        {
            var people = await repo.GetPeople();
            foreach (var person in people)
            {
                Console.WriteLine($"Id: {person.Id}, FName: {person.FirstName}, LName: {person.LastName}, Age: {person.Age}");
            }
        }

        private void ShowZips(IEnumerable<ZipCodeEntity> zips)
        {
            Console.WriteLine("\nZip Codes:");
            foreach (var zip in zips)
            {
                Console.WriteLine($"\t{zip.City}, {zip.State}: {zip.Code}");
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

        private async Task TestReplacingOne(TestMongoContext context)
        {
            var repo = new PeopleRepository(context);
            var joe = await repo.FindPersonByFirstAndLastName("Joe", "Tester");
            var needId = joe.Id;

            var replacePerson = new PersonEntity {Id = needId, FirstName = "Big Bob", LastName = "Blaster", Age = 29};
            var result = await repo.ReplacePerson(replacePerson);
            ShowReplaceOneResult(result);
            await ShowRecords(repo);
        }

        private async Task TestReplacingOneAsUpsert(TestMongoContext context)
        {
            var repo = new PeopleRepository(context);

            var id = ObjectId.GenerateNewId().ToString();
            var replacePerson = new PersonEntity {Id = id, FirstName = "Dandy Andy", LastName = "Master", Age = 39};
            var result = await repo.ReplacePerson(replacePerson);
            ShowReplaceOneResult(result);
            await ShowRecords(repo);
        }

        private void ShowReplaceOneResult(ReplaceOneResult result)
        {
            Console.WriteLine($"\nReplaced Person: Acknowledged = {result.IsAcknowledged}");
            Console.WriteLine($"\tIsModifiedCountAvailable = {result.IsModifiedCountAvailable}");
            if (result.IsModifiedCountAvailable)
                Console.WriteLine($"\tModified Count = {result.ModifiedCount}");
            Console.WriteLine($"\tMatched Count = {result.MatchedCount}");
            Console.WriteLine($"\tUpsertId = {result.UpsertedId}");

        }
    }
}