using System;

namespace Contents.API.Responses.Skill
{
    public class SkillResponse: GenericEntityResponse
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CategoryId { get; set; }

        public Guid PublisherId { get; set; }


        public bool ShouldSerializeCategoryId()
        {
            return CategoryId != Guid.Empty;
        }

        public bool ShouldSerializePublisherId()
        {
            return PublisherId != Guid.Empty;
        }
    }
}
