using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeamSupport.EFData.Models;

namespace TeamSupport.EFData
{
    public class JiraRepository : IJiraRepository, IDisposable
    {
        readonly IGenericRepository<TicketLinkToJira> repository;
        readonly IGenericRepository<CrmLinkTable> crmRepository;

        public JiraRepository(IGenericRepository<TicketLinkToJira> repository)
        {
            this.repository = repository;
        }

        public JiraRepository(IGenericRepository<CrmLinkTable> crmRepository)
        {
            this.crmRepository = crmRepository;
        }

        public IQueryable<TicketLinkToJira> GetAllJiraTickets()
        {
            return repository.GetAll();
        }


        public IQueryable<TicketLinkToJira> GetSingleJiraTicket(int id)
        {
            return repository.FindBy(a => a.id == id);
        }

        public async Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id)
        {
            return await repository.FindByAsync(a => a.id == id).ConfigureAwait(false);
        }

        public void SaveJiraTickets(TicketLinkToJira jiraTickets)
        {
            repository.Add(jiraTickets);
        }

        /// <summary>
        /// Asynchronous save is done at the GenericRepository level
        /// </summary>
        /// <param name="jiraTickets"></param>
        /// <returns>Single Jira Link Ticket</returns>
        public TicketLinkToJira SaveJiraTicketsAsync(TicketLinkToJira jiraTickets)
        {
            return  repository.AddAsync(jiraTickets).Result;
        }

        public void DeleteJiraTicket(TicketLinkToJira jiraTickets)
        {
            repository.Delete(jiraTickets);
        }

        public IQueryable<CrmLinkTable> GetCrmLinkTableById(int? id)
        {
           return crmRepository.FindBy(a => a.CRMLinkID == id);
        }

        public async Task<IQueryable<TicketLinkToJira>> GetTicketsToPushAsIssuesAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<IList<TicketLinkToJira>> GetTicketsToPushAsIssuesAsyncByExpression()
        {
            return await repository.GetAllAsync(IsSynced).ConfigureAwait(false);
        }

        public void DeleteJiraTicketAsync(TicketLinkToJira jiraTicket)
        {
            repository.DeleteAsync(jiraTicket);
        }

        public void UpdateTicket(TicketLinkToJira jiraTicket)
        {
            repository.Edit(jiraTicket);
        }

        public void UpdateTicketAsync(TicketLinkToJira jiraTicket)
        {
            repository.EditAsync(jiraTicket);
        }

        public void Dispose()
        {
            var temp = repository as IDisposable;
            if (temp != null)
                temp.Dispose();
            GC.SuppressFinalize(this);
        }



        Expression<Func<TicketLinkToJira, bool>> IsSynced = a => a.SyncWithJira == true;
    }
}
