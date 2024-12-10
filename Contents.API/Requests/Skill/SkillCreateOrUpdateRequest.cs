using System;
using System.ComponentModel.DataAnnotations;

namespace Contents.API.Requests.Skill
{
    public class SkillCreateOrUpdateRequest
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(65535)]
        public string Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
    }
}
