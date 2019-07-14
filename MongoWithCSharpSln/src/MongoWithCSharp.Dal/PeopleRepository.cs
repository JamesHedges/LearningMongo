using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoWithCSharp.Dal
{
    public class PeopleRepository
    {
        private readonly TestMongoContext _context;

        public PeopleRepository(TestMongoContext context)
        {
            _context = context;
        }

        // Read Methods

        public async Task<IEnumerable<PersonEntity>> GetPeople()
        {
            var people = await _context.People.FindAsync(new BsonDocument());
            
            return people.ToList();
        }

        public async Task<PersonEntity> FindPersonByFirstAndLastName(string firstName, string lastName)
        {
            var fnameFilter = Builders<PersonEntity>.Filter.Eq(p => p.FirstName, firstName);
            var lnameFilter = Builders<PersonEntity>.Filter.Eq(p => p.LastName, lastName);
            var filter = fnameFilter & lnameFilter;
            var person = await _context.People.Find(filter).FirstOrDefaultAsync();

            return person;
        }

        public async Task<PersonEntity> FindPersonByFirstName(string firstName)
        {
            var filter = Builders<PersonEntity>.Filter.Eq(p => p.FirstName, "Jim");
            var person = await _context.People.Find(filter).FirstOrDefaultAsync();

            return person;
        }

        public async Task<PersonEntity> FindPersonByFirstNameWithLinq(string firstName)
        {
            var person = await _context.People.Find(p => p.FirstName == firstName).FirstOrDefaultAsync();

            return person;
        }

        // End of Read Methods

        // Insert Methods

        public async Task InsertAsync(PersonEntity person)
        {
            await _context.People.InsertOneAsync(person);
        }

        public async Task InsertManyAsync(IEnumerable<PersonEntity> people)
        {
            await _context.People.InsertManyAsync(people);
        }

        // End of Insert Methods

        // Replace Methods

        public async Task<ReplaceOneResult> ReplacePerson(PersonEntity person)
        {
            try
            {
            if (string.IsNullOrEmpty(person.Id))
            {
                throw new ArgumentException("Person ID cannot be null.", "Person.Id");
            }

            var filter = Builders<PersonEntity>.Filter.Eq(p => p.Id, person.Id);
            var options = new UpdateOptions { IsUpsert = true };
            //var result = await _context.People.ReplaceOneAsync(filter, person, options);
            var result = await _context.People.ReplaceOneAsync(p => p.Id == person.Id, person, options);

            return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // End of Replace Methods

        // FindOneAndReplace Methods

        public async Task<PersonEntity> FindAndReplacePerson(PersonEntity person)
        {
            try
            {
            if (string.IsNullOrEmpty(person.Id))
            {
                throw new ArgumentException("Person ID cannot be null.", "Person.Id");
            }

            var filter = Builders<PersonEntity>.Filter.Eq(p => p.Id, person.Id);
            var options = new FindOneAndReplaceOptions<PersonEntity> 
            { 
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var result = await _context.People.FindOneAndReplaceAsync(filter, person, options);

            return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        //

        // Update Methods

        public async Task<UpdateResult> UpdatePersonAge(int updateAge, string id)
        {
            try
            {

            var filter = Builders<PersonEntity>.Filter.Eq(p => p.Id, id);
            var update = Builders<PersonEntity>.Update.Set(p => p.Age, updateAge);
            var result = await _context.People.UpdateOneAsync(filter, update);

            return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<PersonEntity> FindOneAndUpdatePersonAge(int updateAge, string id)
        {
            try
            {

            var filter = Builders<PersonEntity>.Filter.Eq(p => p.Id, id);
            var update = Builders<PersonEntity>.Update.Set(p => p.Age, updateAge);
            var options = new FindOneAndUpdateOptions<PersonEntity> { ReturnDocument = ReturnDocument.After };
            var result = await _context.People.FindOneAndUpdateAsync(filter, update, options);

            return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        // End of Update Mehtods
    }
}