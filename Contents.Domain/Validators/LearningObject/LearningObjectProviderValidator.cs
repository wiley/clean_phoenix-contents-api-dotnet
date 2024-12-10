using Contents.Domain.LearningObject;
using System.ComponentModel.DataAnnotations;

namespace Contents.Domain.Validators.LearningObject
{
    public class LearningObjectProviderValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
                if (!LearningObjectProviderEnum.GetTypes().Contains(value.ToString()))
                    return new ValidationResult("Invalid Learning Object Provider.");

            return ValidationResult.Success;
        }
    }
}
