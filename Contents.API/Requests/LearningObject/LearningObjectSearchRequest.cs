using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Contents.API.Requests.LearningObject
{
    public class LearningObjectSearchRequest
    {
        public string Query { get; set; }

        public List<string> Types { get; set; }

        public LearningObjectSearchRequest()
        {
            Types ??= new List<string>();
        }

        public JObject Metadata { get; set; }
    }
}
