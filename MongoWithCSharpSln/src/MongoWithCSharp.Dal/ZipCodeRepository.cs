using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoWithCSharp.Dal
{
    public class ZipCodeRepository
    {
        private readonly TestMongoContext _context;
        private const string ZipsCollectionName = "ZipCodes";

        public ZipCodeRepository(TestMongoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ZipCodeEntity>> LookupCityState(string city, string state)
        {
            var cityRegEx = new BsonRegularExpression($@"/^{Regex.Escape(city)}$/i");
            var stateRegEx = new BsonRegularExpression($@"/^{Regex.Escape(state)}$/i");
            var stateFilter = Builders<ZipCodeEntity>.Filter.Regex(z => z.State, stateRegEx);
            var cityFilter = Builders<ZipCodeEntity>.Filter.Regex(z => z.City, cityRegEx);
            var cityStateFilter = stateFilter & cityFilter;

            return await Lookup(cityStateFilter);
        }

        public async Task<IEnumerable<ZipCodeEntity>> LookupCity(string city)
        {
            var cityRegEx = new BsonRegularExpression($@"/^{Regex.Escape(city)}$/i");
            var cityFilter = Builders<ZipCodeEntity>.Filter.Regex(z => z.City, cityRegEx);
            var sort = Builders<ZipCodeEntity>.Sort.Ascending(z => z.State);

            return await Lookup(cityFilter, sort);
        }

        public async Task<IEnumerable<ZipCodeEntity>> LookupZip(string zip)
        {
            var zipRegEx = new BsonRegularExpression($@"/^{Regex.Escape(zip)}$/i");
            var zipFilter = Builders<ZipCodeEntity>.Filter.Regex(z => z.Code, zipRegEx);

            return await Lookup(zipFilter);
        }

        private async Task<IEnumerable<ZipCodeEntity>> Lookup(FilterDefinition<ZipCodeEntity> filter)
        {
            var zips = await _context.ZipCodes.FindAsync(filter);
            return await zips.ToListAsync();
        }

        private async Task<IEnumerable<ZipCodeEntity>> Lookup(FilterDefinition<ZipCodeEntity> filter, SortDefinition<ZipCodeEntity> sort)
        {
            var zips = _context.ZipCodes.Find(filter).Sort(sort);
            return await zips.ToListAsync();
        }

        public async Task ImportJsonAsync(string filePath)
        {
            try
            {
                await _context.ClearAsync(ZipsCollectionName);
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    await ReadImportStreamAndLoadAsync(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Import Failed: {ex.Message}");
            }
        }

        private async Task ReadImportStreamAndLoadAsync(string filePath)
        {
            using (var streamReader = new StreamReader(filePath))
            {
                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    await LoadJsonAsync(line);
                }
            }
        }

        private async Task LoadJsonAsync(string line)
        {
            using (var jsonReader = new JsonReader(line))
            {
                var context = BsonDeserializationContext.CreateRoot(jsonReader);
                var document = _context.ZipCodes.DocumentSerializer.Deserialize(context);
                //Console.WriteLine($"Document: {document.City}, {document.State}  {document.Code}");
                await _context.ZipCodes.InsertOneAsync(document);
            }
        }
    }
}
