using System;

namespace Contents.API.Responses.Skill
{
    public class SkillCategoryResponse: GenericEntityResponse
    {
        public string Name { get; set; }

        public Guid PublisherId { get; set; }

        public Guid ParentCategoryId { get; set; }


        public bool ShouldSerializePublisherId()
        {
            return PublisherId != Guid.Empty;
        }

        public bool ShouldSerializeParentCategoryId()
        {
            return ParentCategoryId != Guid.Empty;
        }
    }
}
