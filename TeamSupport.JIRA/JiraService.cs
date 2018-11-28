using System;
using System.Collections;
using System.Collections.Generic;
using TeamSupport.Data;
using TeamSupport.EFData;
using TechTalk.JiraRestClient;

namespace TeamSupport.JIRA
{
    public class JiraService : IJiraService
    {
        TechTalk.JiraRestClient.IJiraClient client;
        //Figure out how to get credentials
        public JiraService(TechTalk.JiraRestClient.IJiraClient client)
        {
            this.client = client;
            var baseUrl = @"https://teamsupportio.atlassian.net";
            var username = "jiratest@teamsupport.com";
            var password = "Muroc2008!";
            client = new TechTalk.JiraRestClient.JiraClient(baseUrl, username, password);
        }

        public IEnumerable<TechTalk.JiraRestClient.Issue> GetIssues(string projectKey)
        {
            return client.GetIssues(projectKey);
        }

        //public IEnumerable<Ticket> GetTickets()
        //{
        //    //Get tickets to iterate over comments/attachments/etc.
        //    JiraRepository repo = new JiraRepository()
        //}
    }
}
