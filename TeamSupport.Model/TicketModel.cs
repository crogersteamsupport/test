using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using TeamSupport.Data;

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

        public TicketModel(UserSession user, int ticketID)
        {
            User = user;
            _db = User._db;
            TicketID = ticketID;
            DBReader.VerifyTicket(_db, User.Organization.OrganizationID, TicketID);
        }

        /// <summary> Existing Data.Action </summary>
        public ActionModel Action(int actionID)
        {
            return new ActionModel(this, actionID);
        }

        public ActionModel[] SelectActions()
        {
            //int[] actionIDs = DataAPI.DataAPI.TicketSelectActionIDs(_db, User.Organization.OrganizationID, TicketID);
            //ActionModel[] actions = new ActionModel[actionIDs.Length];
            //for (int i = 0; i < actionIDs.Length; ++i)
            //    actions[i] = new ActionModel(this, actionIDs[i]);
            //return actions;
            return null;
        }

        public void Merge(TicketModel from)
        {
            ActionModel[] actions = from.SelectActions();
            foreach(ActionModel action in actions)
            {
                Merge(action);
            }
        }

        void Merge(ActionModel from)
        {
            //int[] attachmentIDs = DataAPI.DataAPI.ActionAttachmentIDs(_db, User.Organization.OrganizationID, TicketID, from.ActionID);

        }
    }
}
