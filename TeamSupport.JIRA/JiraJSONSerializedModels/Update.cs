using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeamSupport.JIRA.JiraJSONSerializedModels
{
    public class Update
    {
        [JsonProperty("summary")]
        public List<Summary> Summary { get; set; }
        [JsonProperty("description")]
        public List<Description> Description { get; set; }
    }
}
