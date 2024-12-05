using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Contents.Domain.Validators
{
    public class MaxItemsValidator : ValidationAttribute
    {
        private readonly int _maxItems;

        public MaxItemsValidator(int maxItems)
        {
            _maxItems = maxItems;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is ICollection collection && collection.Count > _maxItems)
            {
                return new ValidationResult($"The collection cannot exceed {_maxItems} items.");
            }

            return ValidationResult.Success;
        }
    }
}
