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
<<<<<<< HEAD
        readonly IGenericRepository<TicketLinkToJira> ticketLinkrepository;
        readonly IGenericRepository<CrmLinkTable> crmRepository;
        readonly IGenericRepository<TicketsView> ticketsViewRepository;
        readonly IGenericRepository<Tickets> ticketsRepository;

        public JiraRepository(IGenericRepository<TicketLinkToJira> repository)
        {
            this.ticketLinkrepository = repository;
=======
        readonly IGenericRepository<TicketLinkToJira> repository;
        readonly IGenericRepository<CrmLinkTable> crmRepository;

        public JiraRepository(IGenericRepository<TicketLinkToJira> repository)
        {
            this.repository = repository;
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public JiraRepository(IGenericRepository<CrmLinkTable> crmRepository)
        {
            this.crmRepository = crmRepository;
        }

<<<<<<< HEAD
        public JiraRepository(IGenericRepository<TicketsView> ticketsViewRepository, IGenericRepository<Tickets> ticketsRepository)
        {
            this.ticketsViewRepository = ticketsViewRepository;
            this.ticketsRepository = ticketsRepository;
        }
        
        public TicketsView GetTicketsFromView(int ticketId)
        {
            var tickets = ticketsRepository.FindBy(a=>a.TicketID == ticketId).FirstOrDefault();
            var ticketsView = ticketsViewRepository.FindBy(a => a.OrganizationID == tickets.OrganizationID && a.TicketNumber == tickets.TicketNumber).FirstOrDefault();

            return ticketsView;            
        }

        public IQueryable<TicketLinkToJira> GetAllJiraTickets()
        {
            return ticketLinkrepository.GetAll();
        }

        public IQueryable<TicketLinkToJira> GetSingleJiraTicket(int id)
        {
            return ticketLinkrepository.FindBy(a => a.id == id);
=======
        public IQueryable<TicketLinkToJira> GetAllJiraTickets()
        {
            return repository.GetAll();
        }


        public IQueryable<TicketLinkToJira> GetSingleJiraTicket(int id)
        {
            return repository.FindBy(a => a.id == id);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public async Task<TicketLinkToJira> GetSingleJiraTicketAsync(int id)
        {
<<<<<<< HEAD
            return await ticketLinkrepository.FindByAsync(a => a.id == id).ConfigureAwait(false);
=======
            return await repository.FindByAsync(a => a.id == id).ConfigureAwait(false);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public void SaveJiraTickets(TicketLinkToJira jiraTickets)
        {
<<<<<<< HEAD
            ticketLinkrepository.Add(jiraTickets);
=======
            repository.Add(jiraTickets);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        /// <summary>
        /// Asynchronous save is done at the GenericRepository level
        /// </summary>
        /// <param name="jiraTickets"></param>
        /// <returns>Single Jira Link Ticket</returns>
        public TicketLinkToJira SaveJiraTicketsAsync(TicketLinkToJira jiraTickets)
        {
<<<<<<< HEAD
            return  ticketLinkrepository.AddAsync(jiraTickets).Result;
=======
            return  repository.AddAsync(jiraTickets).Result;
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public void DeleteJiraTicket(TicketLinkToJira jiraTickets)
        {
<<<<<<< HEAD
            ticketLinkrepository.Delete(jiraTickets);
=======
            repository.Delete(jiraTickets);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public IQueryable<CrmLinkTable> GetCrmLinkTableById(int? id)
        {
           return crmRepository.FindBy(a => a.CRMLinkID == id);
        }

        public async Task<IQueryable<TicketLinkToJira>> GetTicketsToPushAsIssuesAsync()
        {
<<<<<<< HEAD
            return await ticketLinkrepository.GetAllAsync().ConfigureAwait(false);
=======
            return await repository.GetAllAsync().ConfigureAwait(false);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public async Task<IList<TicketLinkToJira>> GetTicketsToPushAsIssuesAsyncByExpression()
        {
<<<<<<< HEAD
            return await ticketLinkrepository.GetAllAsync(IsSynced).ConfigureAwait(false);
=======
            return await repository.GetAllAsync(IsSynced).ConfigureAwait(false);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public void DeleteJiraTicketAsync(TicketLinkToJira jiraTicket)
        {
<<<<<<< HEAD
            ticketLinkrepository.DeleteAsync(jiraTicket);
=======
            repository.DeleteAsync(jiraTicket);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public void UpdateTicket(TicketLinkToJira jiraTicket)
        {
<<<<<<< HEAD
            ticketLinkrepository.Edit(jiraTicket);
=======
            repository.Edit(jiraTicket);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public void UpdateTicketAsync(TicketLinkToJira jiraTicket)
        {
<<<<<<< HEAD
            ticketLinkrepository.EditAsync(jiraTicket);
=======
            repository.EditAsync(jiraTicket);
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
        }

        public void Dispose()
        {
<<<<<<< HEAD
            var temp = ticketLinkrepository as IDisposable;
=======
            var temp = repository as IDisposable;
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
            if (temp != null)
                temp.Dispose();
            GC.SuppressFinalize(this);
        }
<<<<<<< HEAD
               
        Expression<Func<TicketLinkToJira, bool>> IsSynced = a => a.SyncWithJira == true;

=======



        Expression<Func<TicketLinkToJira, bool>> IsSynced = a => a.SyncWithJira == true;
>>>>>>> c364012ad236c68b013e5deb1570f4103397046d
    }
}
