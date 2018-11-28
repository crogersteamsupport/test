using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.JIRA.JiraJSONSerializedModels
{
    public class BaseIssue
    {
        public string ProjectKey { get; set; }
        public string IssueType { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
    }
}
