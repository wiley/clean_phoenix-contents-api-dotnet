using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Contents.Domain.TrainingProgram
{
    [BsonCollection("training_program")]
    [BsonIgnoreExtraElements]
    public class TrainingProgram : GenericEntity
    {
        [BsonIgnoreIfNull]
        public string Title { get; set; }

        [BsonIgnoreIfNull]
        public string Description { get; set; }

        public int EstimatedDuration { get; set; }

        [BsonIgnoreIfNull]
        public string LanguageTag { get; set; }

        [BsonIgnoreIfNull]
        public string ReferenceCode { get; set; }

        [BsonIgnoreIfNull]
        public string ThumbnailPath { get; set; }

        [BsonIgnoreIfNull]
        public List<int> OrganizationIds { get; set; }

        [BsonIgnoreIfNull]
        public List<Guid> ProductIds { get; set; }

        [BsonIgnoreIfNull]
        public bool IsDiscoverable { get; set; }

        [BsonIgnoreIfNull]
        public List<string> Tags { get; set; }

        [BsonIgnoreIfNull]
        public List<string> Authors { get; set; }

        [BsonIgnoreIfNull]
        public List<TrainingProgramStep> Steps { get; set; }

        public TrainingProgram()
        {
            OrganizationIds ??= new List<int>();
            ProductIds ??= new List<Guid>();
            Tags ??= new List<string>();
            Authors ??= new List<string>();
            Steps ??= new List<TrainingProgramStep>();
        }

    }
}
