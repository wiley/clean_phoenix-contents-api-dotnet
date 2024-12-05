using System;
using System.ComponentModel.DataAnnotations;
using Contents.Domain.Utils.CustomValidations;
using Newtonsoft.Json;

namespace Contents.API.Requests.TrainingProgram
{
    public class TrainingProgramStepLOCreateRequest
    {
        public LearningObjectIdCreateRequest LearningObject { get; set; }
        
        [JsonConverter(typeof(CustomBooleanConverterFormatValidation))]
        public bool IsMandatory { get; set; } = false;
    }
    
    public class LearningObjectIdCreateRequest
    {
        public Guid Id { get; set; }
    }

}
