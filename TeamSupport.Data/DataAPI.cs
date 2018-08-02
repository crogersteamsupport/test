using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace TeamSupport.Data
{
    public class DataAPI
    {
        public static void VerifyOrganization(DataContext db, int organizationID)
        {
            Verify(db, $"SELECT OrganizationID FROM Organizations WITH (NOLOCK) WHERE OrganizationID={organizationID}");
        }

        public static void VerifyUser(DataContext db, int organizationID, int userID)
        {
            string query = $"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={userID} AND OrganizationID={organizationID}";
        }

        public static void VerifyTicket(DataContext db, int organizationID, int ticketID)
        {
            Verify(db, $"SELECT TicketID FROM Tickets WITH (NOLOCK) WHERE TicketID={ticketID} AND OrganizationID={organizationID}");
        }

        public static void VerifyAction(DataContext db, int organizationID, int ticketID, int actionID)
        {
            Verify(db, $"SELECT ActionID FROM Actions WITH (NOLOCK) WHERE ActionID={actionID} AND TicketID={ticketID}");
        }

        public static void VerifyActionAttachment(DataContext db, int organizationID, int ticketID, int actionID, int actionAttachmentID)
        {
            Verify(db, $"SELECT AttachmentID FROM ActionAttachments WITH (NOLOCK) WHERE AttachmentID={actionAttachmentID} AND ActionID={actionID} AND OrganizationID={organizationID}");
        }

        private static void Verify(DataContext db, string query)
        {
            if (!db.ExecuteQuery<int>(query).Any()) // valid ID found?
                throw new Exception(String.Format($"{query} not found"));
        }

        public static void InsertActionAttachment(DataContext db, int ticketID, ref AttachmentProxy proxy)
        {
            int organizationID = proxy.OrganizationID;
            int actionID = proxy.RefID;

            // hard code all the numbers, parameterize all the strings so they are SQL-Injection checked
            string query = "INSERT INTO ActionAttachments(OrganizationID, FileName, FileType, FileSize, Path, DateCreated, DateModified, CreatorID, ModifierID, ActionID, SentToJira, SentToTFS, SentToSnow, FilePathID) " +
                $"VALUES({organizationID}, {{0}}, {{1}}, {proxy.FileSize}, {{2}}, '{proxy.DateCreated}', '{proxy.DateModified}', {proxy.CreatorID}, {proxy.ModifierID}, {actionID}, {(proxy.SentToJira ? 1 : 0)}, {(proxy.SentToTFS ? 1 : 0)}, {(proxy.SentToSnow ? 1 : 0)}, {proxy.FilePathID})" +
                "SELECT SCOPE_IDENTITY()";
            decimal value = db.ExecuteQuery<decimal>(query, proxy.FileName, proxy.FileType, proxy.Path).Min();
            proxy.AttachmentID = Decimal.ToInt32(value);
        }

        public static void DeleteActionAttachment(LoginUser loginUser, int organizationID, int ticketID, int actionID, int attachmentID)
        {
            // set WITH (ROWLOCK) 
            Attachment attachment = Attachments.GetAttachment(loginUser, attachmentID);
            attachment.DeleteFile();
            attachment.Delete();
            attachment.Collection.Save();
        }

        public static int[] SelectActionIDs(DataContext db, int organizationID, int ticketID)
        {
            return db.ExecuteQuery<int>($"SELECT ActionID FROM Actions a WHERE a.TicketID={ticketID}").ToArray();
        }

        /// <summary> extracted from ts-app\WebApp\App_Code\TicketPageService.cs UpdateAction(ActionProxy proxy) </summary>
        public static ActionProxy InsertAction(LoginUser loginUser, ActionProxy proxy, DataContext db)
        {
            Action action = (new Actions(loginUser)).AddNewAction();
            action.TicketID = proxy.TicketID;
            action.CreatorID = loginUser.UserID;
            action.Description = proxy.Description;

            // add signature?
            string signature = db.ExecuteQuery<string>($"SELECT [Signature] FROM Users WITH (NOLOCK) WHERE UserID={loginUser.UserID} AND OrganizationID={loginUser.OrganizationID}").FirstOrDefault();    // 175915
            if (!string.IsNullOrWhiteSpace(signature) && proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase && proxy.ActionID == -1 &&
                (!proxy.Description.Contains(signature)))
            {
                action.Description += "<br/><br/>" + signature;
            }

            action.ActionTypeID = proxy.ActionTypeID;
            action.DateStarted = proxy.DateStarted;
            action.TimeSpent = proxy.TimeSpent;
            action.IsKnowledgeBase = proxy.IsKnowledgeBase;
            action.IsVisibleOnPortal = proxy.IsVisibleOnPortal;
            action.Collection.Save();
            return action.GetProxy();
        }

        /// <summary> extracted from ts-app\webapp\app_code\ticketservice.cs </summary>
        public static ActionProxy InsertAction(Ticket ticket, ActionProxy proxy, User user)
        {
            Action action = (new Actions(ticket.Collection.LoginUser)).AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Description";
            action.SystemActionTypeID = SystemActionType.Description;
            action.ActionSource = ticket.TicketSource;
            action.Description = proxy.Description;


            if (!string.IsNullOrWhiteSpace(user.Signature) && proxy.IsVisibleOnPortal)
            {
                action.Description = action.Description + "<br/><br/>" + user.Signature;
            }

            action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
            action.IsKnowledgeBase = ticket.IsKnowledgeBase;
            action.TicketID = ticket.TicketID;
            action.TimeSpent = proxy.TimeSpent;
            action.DateStarted = proxy.DateStarted;
            action.Collection.Save();
            return action.GetProxy();
        }

    }
}
