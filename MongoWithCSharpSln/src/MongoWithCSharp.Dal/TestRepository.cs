using System;
using System.Collections.Generic;
//using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoWithCSharp.Dal
{
    public class TestRepository
    {
        private readonly TestMongoContext _context;

        public TestRepository(TestMongoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonEntity>> GetPeople()
        {
            var people = await _context.People.FindAsync(new BsonDocument());
            
            return people.ToList();
        }

        public async Task<PersonEntity> FindPersonByFirstName(string firstName)
        {
            var filter = Builders<PersonEntity>.Filter.Eq(p => p.FirstName, "Jim");
            var person = await _context.People.Find(filter).FirstOrDefaultAsync();

            return person;
        }

        public async Task InsertAsync(PersonEntity person)
        {
            await _context.People.InsertOneAsync(person);
        }

        public async Task InsertManyAsync(IEnumerable<PersonEntity> people)
        {
            await _context.People.InsertManyAsync(people);
        }
    }
}