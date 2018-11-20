using Newtonsoft.Json;

namespace TeamSupport.JIRA.JiraJSONSerializedModels
{
    public class Description
    {
        [JsonProperty(PropertyName ="set")]
        public string Set { get; set; }
    }
}
