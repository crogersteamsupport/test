using Newtonsoft.Json;

namespace TeamSupport.JIRA.JiraJSONSerializedModels
{
    public class Summary
    {
        [JsonProperty("set")]
        public string Set { get; set; }
    }
}
