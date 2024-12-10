using Newtonsoft.Json;
using System;

namespace Contents.API.Responses
{
    public abstract class GenericEntityResponse
    {
        [JsonProperty(Order = -2)]
        public Guid Id { get; set; }

        [JsonProperty(Order = 100)]
        public int CreatedBy { get; set; }

        [JsonProperty(Order = 101)]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(Order = 102)]
        public int UpdatedBy { get; set; }

        [JsonProperty(Order = 103)]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty(Order = 104)]
        public LinkSelfResponse _links { get; set; }

        public GenericEntityResponse()
        {
            _links ??= new LinkSelfResponse();
        }
    }
}
