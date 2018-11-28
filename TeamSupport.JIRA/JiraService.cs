using System.Linq;
using TeamSupport.EFData;
using TeamSupport.EFData.Models;
using TicketsView = TeamSupport.EFData.Models.TicketsView;

namespace TeamSupport.JIRA
{
    public class JiraService 
    {

        public TicketsView GetTicketData(int ticketId)
        {
            var ticketData = new TicketsView();
            using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<TicketsView>(), new GenericRepository<EFData.Models.Tickets>())))
            {
                ticketData = jiraTicketsService.GetTicketsFromViewAsync(ticketId);
            }
            return ticketData;
        }

        public CrmLinkTable GetCRMLinkTableData(int? crmLinkId)
        {
            var crmLinkTable = new CrmLinkTable();
            using (var jiraTicketsService = new JiraTicketsService(new JiraRepository(new GenericRepository<CrmLinkTable>())))
            {
                crmLinkTable = jiraTicketsService.GetCrmLinkTableById(crmLinkId).FirstOrDefault();
            }
            return crmLinkTable;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="rootObject">Should contain ProjetKey, IssueType, and Summary</param>
        /// <returns></returns>
        public TechTalk.JiraRestClient.Issue CreateNewJiraTicket(string baseUrl, string username, string password, string projectKey, string issueType, TechTalk.JiraRestClient.IssueFields issueFields)
        {
            issueFields.timetracking = null;//necessary for API rest call
            var response = new TechTalk.JiraRestClient.Issue();
            var myClient = new TechTalk.JiraRestClient.JiraClient(baseUrl, username, password);
            if (issueFields != null)
            {
                response = myClient.CreateIssue(projectKey, issueType, issueFields);
                if (response == null)
                {
                    return new TechTalk.JiraRestClient.Issue();
                }
            }

            return response;
        }
    }
}
