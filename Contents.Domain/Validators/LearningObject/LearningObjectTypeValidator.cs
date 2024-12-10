using Contents.Domain.LearningObject;
using System.ComponentModel.DataAnnotations;

namespace Contents.Domain.Validators.LearningObject
{
    public class LearningObjectTypeValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
                if (!LearningObjectTypeEnum.GetTypes().Contains(value.ToString()))
                    return new ValidationResult("Invalid Learning Object Type.");

            return ValidationResult.Success;
        }
    }
}
