using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.JIRA.JiraJSONSerializedModels
{
    public class Object
    {
        public string url { get; set; }
        public string title { get; set; }
        public Icon icon { get; set; }
        public Status status { get; set; }
    }

}
