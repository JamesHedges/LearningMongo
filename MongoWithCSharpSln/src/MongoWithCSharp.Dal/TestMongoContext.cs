using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoWithCSharp.Dal
{
    public class TestMongoContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;
        private static TestMongoContext TestContext;
        private const string DatabaseName = "Test";

        private TestMongoContext(string connectionString)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(DatabaseName);
        }
        
        public static TestMongoContext Create(string connectionString)
        {
            if (TestContext == null)
            {
                TestContext = new TestMongoContext(connectionString);
            }
            return TestContext;
        }

        public IMongoCollection<PersonEntity> People
        {
            get
            {
                return _database.GetCollection<PersonEntity>("People");
            }
        }

        public async Task ClearAsync(string collectionName)
        {
            await _database.DropCollectionAsync(collectionName);
        }
    }
}