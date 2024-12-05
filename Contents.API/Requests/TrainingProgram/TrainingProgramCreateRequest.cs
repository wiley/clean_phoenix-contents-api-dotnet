using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contents.API.Requests.TrainingProgram
{
    public class TrainingProgramCreateRequest
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(65535)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
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
        public bool IsDiscoverable { get; set; } = true;

        [BsonIgnoreIfNull]
        public List<string> Tags { get; set; }

        [BsonIgnoreIfNull]
        public List<string> Authors { get; set; }
    }
}
