using Contents.Domain.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace Contents.Domain
{

    public abstract class GenericEntity : IDocument
    {

        [BsonId(IdGenerator = typeof(GuidGenerator))]
        public Guid Id { get; set; }

        [BsonRequired]
        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        [BsonRequired]
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}