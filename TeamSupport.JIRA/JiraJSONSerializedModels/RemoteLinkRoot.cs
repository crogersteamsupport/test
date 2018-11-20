using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.JIRA.JiraJSONSerializedModels
{
    public class RemoteLinkRoot
    {
        public int id { get; set; }
        public string self { get; set; }
        public Application application { get; set; }
        public Object @object { get; set; }
    }
}
