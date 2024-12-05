using Contents.Domain.Validators.LearningObject;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contents.Domain.LearningObject
{
    public class LearningObjectFilter
    {
        [Range(0, int.MaxValue)]
        public int Offset { get; set; } = 0;

        [Range(1, 50)]
        public int Size { get; set; } = 20;

        public string LanguageTag { get; set; }

        [LearningObjectTypeValidator]
        public string Type { get; set; }

        [FromQuery(Name = "estimatedDuration.equal")]
        public int? EstimatedDurationEqual { get; set; }

        [FromQuery(Name = "estimatedDuration.greaterThan")]
        public int? EstimatedDurationGreaterThan { get; set; }

        [FromQuery(Name = "estimatedDuration.lowerThan")]
        public int? EstimatedDurationLowerThan { get; set; }

        [FromQuery(Name = "estimatedDuration.greaterThanEqual")]
        public int? EstimatedDurationGreaterThanEqual { get; set; }

        [FromQuery(Name = "estimatedDuration.lowerThanEqual")]
        public int? EstimatedDurationLowerThanEqual { get; set; }
    }
}
