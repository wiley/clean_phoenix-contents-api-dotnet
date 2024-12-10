using Contents.Domain.LearningObject;
using System.ComponentModel.DataAnnotations;

namespace Contents.Domain.Validators.LearningObject
{
    public class LearningObjectRuntimeValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
                if (!LearningObjectRuntimeEnum.GetTypes().Contains(value.ToString()))
                    return new ValidationResult("Invalid Learning Object Runtime.");

            return ValidationResult.Success;
        }
    }
}
