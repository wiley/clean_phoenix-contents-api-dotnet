using System;
using System.ComponentModel.DataAnnotations;

namespace Contents.API.Requests.SkillCategory
{
    public class SkillCategoryCreateOrUpdateRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public Guid PublisherId { get; set; }

        public Guid ParentCategoryId { get; set; }
    }
}
