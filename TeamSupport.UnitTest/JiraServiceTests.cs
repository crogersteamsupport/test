using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.JIRA;

namespace TeamSupport.UnitTest
{
    /// <summary>
    /// Only mock dependencies!
    /// </summary>
    [TestFixture]
    public class JiraServiceTests
    {
        //Can do minimal init for :Task, Story, New Feature, Bugs, 

       [Test]
       public void TicketService_ShouldSendIssueToJira_Successfully()
       {
            var token = @"amlyYXRlc3RAdGVhbXN1cHBvcnQuY29tOk11cm9jMjAwOCE=";//Base64 encoded username:password pattern gets back single-string token
            var baseURL = @"https://teamsupportio.atlassian.net";
            var myClient = new TechTalk.JiraRestClient.JiraClient(baseURL, "jiratest@teamsupport.com", "Muroc2008!");
            var issueTypes = myClient.GetIssueTypes();
            // var issueLinks =  myClient.GetIssueLinks(new TechTalk.JiraRestClient.IssueRef() {  key = "SSP-171" });
            try
            {
                //timetracking must be sent to some value
                var customField = new Dictionary<int, object>();
                    
                var issueFields = new TechTalk.JiraRestClient.IssueFields() { summary = "Name of thing", timetracking = null};
                    var actual = myClient.CreateIssue("SSP", "Epic", issueFields);
                foreach (var issueType in issueTypes)
                {
                    if(!issueType.name.Contains("Sub"))
                        Assert.DoesNotThrow(() => myClient.CreateIssue("SSP", issueType.name, issueFields));
                }
            }
            catch(Exception ex)
            {
                var result = ex.Message;
            }
       }



    }
}
