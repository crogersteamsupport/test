﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamSupport.EFData.Models;

namespace TeamSupport.JIRA
{
    public interface IJiraTicketsService
    {
        void DeleteJiraTicket(TicketLinkToJira jiraTickets);
        void DeleteJiraTicketAsync(TicketLinkToJira jiraTickets);
        List<TicketLinkToJira> GetTicketsToPushAsIssues();
        TicketLinkToJira GetTicketsToPushAsIssuesById(int id);
        void SaveJiraTickets(TicketLinkToJira jiraTickets);
        void SaveJiraTicketsAsync(TicketLinkToJira jiraTickets);
        void UpdateTicket(TicketLinkToJira jiraTicket);
        void UpdateTicketAsync(TicketLinkToJira jiraTicket);
        Task<IQueryable<TicketLinkToJira>> GetTicketsToPushAsIssuesAsync();
        Task<IList<TicketLinkToJira>> GetTicketsToPushAsIssuesAsyncByExpression();
        Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id);
        IQueryable<CrmLinkTable> GetCrmLinkTableById(int? id);
        TicketsView GetTicketsFromViewAsync(int ticketId);
    }
}