using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using TeamSupport.Data;
using System.Data;

namespace TeamSupport.Data.BusinessObjects
{
    /// <summary>
    /// Linq class to integrate with ActionToAnalyze Table
    /// </summary>
    [Table(Name = "ActionToAnalyze")]
    class ActionToAnalyze
    {
        private Int64 _ActionToAnalyzeID;
        [Column(Storage = "_ActionToAnalyzeID", IsPrimaryKey = true, IsDbGenerated = true)]
        public Int64 ActionToAnalyzeID { get { return _ActionToAnalyzeID; } }

        [Column]
        public int ActionID;
        [Column]
        public int TicketID;
        [Column]
        public int UserID;
        [Column]
        public int OrganizationID;
        [Column]
        public bool IsAgent;
        [Column]
        public DateTime DateCreated;
        [Column]
        public string ActionDescription;

        /// <summary> 
        /// Previously the watson service used Stored Procedure dbo.ActionsGetForWatson 
        /// to create records in Table ActionToAnalyze
        /// </summary>
        /// <param name="a"></param>
        public static void QueueForWatsonToneAnalysis(Action a, SqlConnection connection, LoginUser user)
        {
            try
            {
                //--------- Queue for IBM Watson?
                int creatorID = a.CreatorID;   // JOIN dbo.Users creator WITH(NOLOCK) ON a.[creatorid] = creator.[userid]
                if (a.CreatorID == 0)
                    creatorID = user.UserID;   //  ? first ticket on first action has CreatorID == 0 ?
                LoginUser creator = user;

                int ticketID = a.TicketID;  // JOIN dbo.Tickets t WITH (NOLOCK) ON a.[ticketid] = t.[ticketid]
                Ticket t = Tickets.GetTicket(creator, ticketID);

                int accountID = t.OrganizationID;   // JOIN dbo.Organizations account WITH (NOLOCK) ON t.organizationid = account.organizationid
                Organization account = Organizations.GetOrganization(creator, accountID);

                //account.[usewatson] = 1
                //AND a.[isvisibleonportal] = 1
                //AND t.[isvisibleonportal] = 1
                //AND account.producttype = 2
                // AND NOT EXISTS (SELECT NULL FROM ActionSentiments ast WHERE a.actionid = ast.actionid);
                if (!(account.UseWatson &&
                    a.IsVisibleOnPortal && t.IsVisibleOnPortal &&
                    (account.ProductType == ProductType.Enterprise)))
                    return;

                //CASE
                //    WHEN creatorCompany.organizationid = account.organizationid THEN 1
                //    ELSE 0
                //END AS[IsAgent]
                int creatorCompanyID = creator.OrganizationID;  // JOIN dbo.Organizations creatorCompany WITH(NOLOCK) ON creatorCompany.[organizationid] = creator.[organizationid]
                Organization creatorCompany = Organizations.GetOrganization(creator, creatorCompanyID);

                // insert a record into the Table dbo.ActionToAnalyze
                ActionToAnalyze actionToAnalyze = new ActionToAnalyze
                {
                    ActionID = a.ActionID,
                    TicketID = a.TicketID,
                    UserID = creatorID,
                    OrganizationID = creator.OrganizationID,
                    IsAgent = creatorCompany.OrganizationID == account.OrganizationID,
                    ActionDescription = a.Description,
                    DateCreated = a.DateCreated
                };

                // SUCCESS! create the ActionToAnalyze
                using (DataContext db = new DataContext(connection))
                {
                    Table<ActionToAnalyze> actionToAnalyzeTable = db.GetTable<ActionToAnalyze>();
                    actionToAnalyzeTable.InsertOnSubmit(actionToAnalyze);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
