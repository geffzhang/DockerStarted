using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entities
{
    public class Person
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
