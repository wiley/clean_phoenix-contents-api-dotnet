using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Contents.API.Responses.LearningObject
{
    public class LearningObjectResponse: GenericEntityResponse
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? EstimatedDuration { get; set; }

        public string LanguageTag { get; set; }

        public double? Version { get; set; }

        public string Runtime { get; set; }

        public string ReferenceCode { get; set; }

        public string Provider { get; set; }

        public List<string> Tags { get; set; }

        public List<string> Authors { get; set; }

        public JObject Metadata { get; set; }

        public string ThumbnailPath { get; set; }

        public List<int> OrganizationIds { get; set; }

        public List<Guid> ProductIds { get; set; }

        public bool IsDiscoverable { get; set; }
    }
}
