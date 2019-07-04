using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoWithCSharp.Dal
{
    public class DataContext<T> where T: class
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;
        //private readonly MongoServer _server;
        private readonly MongoClient _client;

        public DataContext(string connectionString, string databaseName, string collectionName)
        {
            _client = new MongoClient(connectionString);
            //_server = _client.GetServer();
            _database = _client.GetDatabase(databaseName);
            _collectionName = collectionName;
        }

        public IMongoCollection<T> GetCollection()
        {
            return _database.GetCollection<T>(_collectionName);
        }

        public async Task InsertAsync(T entity)
        {
            var collection = GetCollection();
            await collection.InsertOneAsync(entity);
        }

        public async Task InsertManyAsync(IEnumerable<T> entities)
        {
            var collection = GetCollection();
            await collection.InsertManyAsync(entities);
        }

        public async Task ClearAsync()
        {
            await _database.DropCollectionAsync(_collectionName);
        }
    }
}