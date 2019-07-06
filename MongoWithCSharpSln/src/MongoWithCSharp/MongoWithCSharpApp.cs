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

            var person = await repo.FindPersonByFirstNameWithLinq("Jim");
            Console.WriteLine($"\nFound: {person.FirstName} {person.LastName}");

            var person2 = await repo.FindPersonByFirstAndLastName("Anna", "Hedges");
            Console.WriteLine($"\nFound: {person2.FirstName} {person2.LastName}");

            var zipRepo = new ZipCodeRepository(context);
            //await LoadZipCodeData(zipRepo);
            //var zips = await zipRepo.LookupCityState("peoria", "il");
            //var zips = await zipRepo.LookupCity("dallas");
            var zips = await zipRepo.LookupCityLinq("dallas");
            //var zips = await zipRepo.LookupZip("87107");
            ShowZips(zips);
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

        private async Task ShowRecords(TestRepository repo)
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
    }
}