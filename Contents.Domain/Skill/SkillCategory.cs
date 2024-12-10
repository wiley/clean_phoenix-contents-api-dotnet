using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Contents.Domain.Skill
{
    [BsonCollection("skill_category")]
    [BsonIgnoreExtraElements]
    public class SkillCategory : GenericEntity
    {
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [BsonIgnoreIfNull]
        public Guid PublisherId { get; set; }

        [BsonIgnoreIfNull]
        public Guid ParentCategoryId { get; set; }

    }
}
