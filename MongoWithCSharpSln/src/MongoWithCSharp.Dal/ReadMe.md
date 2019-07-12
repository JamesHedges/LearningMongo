# Mongo Data Access
## Context
The context is a singleton class for managing the database connection and its collections.
* Since database connections (through the MongoClient) are threadsafe, it can be implemented as a singleton and shared across threads
* Unlike EF, table mapping classes are not needed. The collections are simply mapped through the document (entity), set by the generic parameter. Note that mappings can be configured using attributes on the poco or by registering them. To register, at startup, use BsonClassMap.RegisterClass.
* The client acts as the "server" object. It wraps a MongoServer. It can be used to manage collections
## Repository
The repository is for manipulating and accessing a database's collection.
* One repository per collection
* For now... contains the queries for all a database's collections. These should be moved to query objects and the repository should be associated with a domain's aggregate root.
* Handles all of the collection's insert/update/delete operations
### TestRepository
* Very simple collection with
* Data loaded by the application
* Not high volumn
### Zip Code Repository
* Zip code by city, state for US
* Populated by loading from a json file
  * See ImportJsonAsync method
  * Uses stream reader to get rows from the file
  * Uses BsonSerializer to create collection's poco (ZipCodeEntity)
  * Uses insert statement to add record
* Contains over 29,000 rows
## Entities
Entities are the poco classes mapped to the collections.
* One entity per collection
* An entity may contain child entities (Entity != Collection)
* Mapping entities is not required. Can always use a BsonDocument to generically map them to a dynamic structure
### Person Entity
* Used with the Test collection
* Fields
  * First Name
  * Last Name
  * Age

### Zip Code Entity