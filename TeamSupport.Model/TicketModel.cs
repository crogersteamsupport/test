﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace TeamSupport.Model
{
    /// <summary>
    /// Wrapper for valid TicketID
    /// </summary>
    public class TicketModel
    {
        public UserSession User { get; private set; }
        public int TicketID { get; private set; }
        public DataContext _db { get; private set; }
        public Customer Customer { get; private set; }

        public TicketModel(UserSession user, int ticketID)
        {
            User = user;
            _db = User._db;
            TicketID = ticketID;
            Verify();
        }

        [Conditional("DEBUG")]
        void Verify()
        {
            string query = $"SELECT TicketID FROM Tickets  WITH (NOLOCK) WHERE TicketID={TicketID} AND OrganizationID={User.Organization.OrganizationID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        /// <summary> Existing Data.Action </summary>
        public ActionModel Action(int actionID)
        {
            return new ActionModel(this, actionID);
        }

        /// <summary> Create new Data.Action on an existing ticket </summary>
        public ActionModel InsertAction(Data.LoginUser loginUser, Data.ActionProxy proxy)
        {
            return new ActionModel(this, loginUser, proxy);
        }

        /// <summary> Create new Data.Action on new ticket </summary>
        public ActionModel InsertAction(Data.ActionProxy info, Data.Ticket ticketData, Data.User user)
        {
            return new ActionModel(this, info, ticketData, user);
        }

        /// <summary>
        /// equivalent to ts-app\TeamSupport.Data\BusinessObjects\Tickets.cs MergeAttachments(int oldticketID, int newticketID)
        /// </summary>
        public void MergeAttachments(int oldticketID)
        {
            // take attachments
            string query = $"UPDATE attachments SET RefID={TicketID} WHERE (RefID = {oldticketID} AND RefType = {Data.ReferenceType.Actions}";
            _db.ExecuteCommand(query);

            // log old ticket number
            query = $"SELECT TicketNumber FROM Tickets WHERE TicketID={oldticketID}";
            int ticketNumber = _db.ExecuteQuery<int>(query).FirstOrDefault();
            User.AddActionLog(Data.ActionLogType.Update, Data.ReferenceType.Tickets, TicketID, $"Merged '{ticketNumber}' Data.Action Attachments");
        }
    }
}
