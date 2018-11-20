using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeamSupport.JIRA
{
    public class RemoteLinkAbbreviated
    {
        [JsonProperty("object")]
        public Dictionary<string, string> Object{get;set;}
    }
}