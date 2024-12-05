using Contents.Domain.TrainingProgram;
using System;
using System.Collections.Generic;

namespace Contents.API.Responses.TrainingProgram
{
    public class TrainingProgramResponse : GenericEntityResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int EstimatedDuration { get; set; }
        public string LanguageTag { get; set; }
        public string ReferenceCode { get; set; }
        public string ThumbnailPath { get; set; }
        public List<int> OrganizationIds { get; set; }
        public List<Guid> ProductIds { get; set; }
        public bool IsDiscoverable { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Authors { get; set; }
        public List<TrainingProgramStepSimplifiedResponse> Steps { get; set; }

        public bool ShouldSerializeSteps()
        {
            return Steps.Count > 0;
        }
    }
}
