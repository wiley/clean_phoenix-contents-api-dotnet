using System.ComponentModel.DataAnnotations;

namespace Contents.Domain.Skill
{
    public class SkillCategoryFilter
    {
        [Range(0, int.MaxValue)]
        public int Offset { get; set; } = 0;

        [Range(0, 50)]
        public int Size { get; set; } = 20;

        public bool RootOnly { get; set; } = false;
    }
}
