using Contents.Domain.Validators;
using Contents.Domain.Validators.LearningObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contents.API.Requests.LearningObject
{
    public class LearningObjectCreateRequest
    {
        [Required]
        [LearningObjectTypeValidator]
        public string Type { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }


        [MaxLength(65535)]
        public string Description { get; set; }

        
        public int? EstimatedDuration { get; set; }

        [Required]
        [MaxLength(20)]
        public string LanguageTag { get; set; }

        
        public double? Version { get; set; }

        [Required]
        [LearningObjectRuntimeValidator]
        public string Runtime { get; set; }

        [MaxLength(255)]
        public string ReferenceCode { get; set; }

        [Required]
        [LearningObjectProviderValidator]
        public string Provider { get; set; }

        
        public List<string> Tags { get; set; }

        
        public List<string> Authors { get; set; }

        public JObject Metadata { get; set; }

        
        public string ThumbnailPath { get; set; }


        [MaxItemsValidator(20)]
        public List<int> OrganizationIds { get; set; }

        [MaxItemsValidator(20)]
        public List<Guid> ProductIds { get; set; }


        public bool IsDiscoverable { get; set; } = true;
    }
}
