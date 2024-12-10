using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Contents.Domain.Utils.CustomValidations
{
    public class CustomIdFormatValidation : ValidationAttribute
    {
        private static readonly string INVALID_ID_FORMAT = "The Id is in wrong format.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isValid = Guid.TryParse(value.ToString(), out Guid pattern);

            if (!isValid)
                return new ValidationResult(INVALID_ID_FORMAT);

            return ValidationResult.Success;
        }

    }

    public class CustomBooleanConverterFormatValidation : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value;

            if (value.GetType() != typeof(bool))
            {
                throw new JsonReaderException("The value is not boolean.");
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value as string);
        }



    }
}
