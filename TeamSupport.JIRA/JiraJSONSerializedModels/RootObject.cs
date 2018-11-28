using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.JIRA.JiraJSONSerializedModels
{
<<<<<<< HEAD
    public class BaseIssue
    {
        public string ProjectKey { get; set; }
        public string IssueType { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
=======
    public class RootObject
    {
        public Fields fields { get; set; }
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
    }
}
