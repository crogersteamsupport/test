using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web;
using System.Diagnostics;
using TeamSupport.Proxy;
using TeamSupport.Data;
using System.Web.Security;

namespace TeamSupport.Model
{
    /// <summary> Action Attachments </summary>
    public class ActionAttachment
    {
        public ActionModel Action { get; private set; }
        public int? ActionAttachmentID { get; private set; }
        public DataContext _db { get; private set; }

        /// <summary> Load existing action attachment /// </summary>
        public ActionAttachment(ActionModel action, int actionAttachmentID)
        {
            Action = action;
            ActionAttachmentID = actionAttachmentID;
            _db = action._db;

            TicketModel ticket = Action.Ticket;
            OrganizationModel organization = ticket.User.Organization;
            DBReader.VerifyActionAttachment(_db, organization.OrganizationID, ticket.TicketID, Action.ActionID, ActionAttachmentID.Value);
        }

    }
}

