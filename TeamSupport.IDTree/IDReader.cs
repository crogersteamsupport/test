using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TeamSupport.IDTree
{
    public enum OrganizationChild
    {
        OrganizationTicket,
        Tag,
        Task,
        Ticket,
        User
    }

    public enum TicketChild
    {
        Action,
        AssetTicket,
        Asset,
        Children,
        Contacts,
        Customers,
        TicketReminders,
        Subscriptions,
        TicketTagLinks,
        TaskAssociations,
    }

    public enum TicketAssociation
    {
        QueueUsers,
        Relationships1,
        Relationships2,
    }


    /// <summary>
    /// ultralight read-only interface to database for verifying primary keys
    /// </summary>
    public static class IDReader
    {
        // Verify row correctness knowing the associated model is also internally correct
        public static void VerifyOrganization(DataContext db, int organizationID)
        {
            Verify(db, $"SELECT OrganizationID FROM Organizations WITH (NOLOCK) WHERE OrganizationID={organizationID}");
        }

        public static void Verify(OrganizationChild organizationChild, DataContext db, int organizationID, int id)
        {
            string query = String.Empty;
            switch(organizationChild)
            {
                case OrganizationChild.OrganizationTicket:
                    query = $"SELECT OrganizationID FROM OrganizationTickets WITH (NOLOCK) WHERE TicketID={id} AND OrganizationID={organizationID}";
                    break;
                case OrganizationChild.Tag:
                    query = $"SELECT TagID FROM Tags WITH (NOLOCK) WHERE TagID={id} && OrganizationID={organizationID}";
                    break;
                case OrganizationChild.Ticket:
                    query = $"SELECT TicketID FROM Tickets WITH (NOLOCK) WHERE TicketID={id} AND OrganizationID={organizationID}";
                    break;
                case OrganizationChild.User:
                    query = $"SELECT UserID FROM Users WITH (NOLOCK) WHERE UserID={id} AND OrganizationID={organizationID}";
                    break;
                case OrganizationChild.Task:
                    query = $"SELECT TaskID FROM Tasks WITH (NOLOCK) WHERE TaskID={id} AND OrganizationID={organizationID}";
                    break;
            }
            Verify(db, query);
        }

        public static void Verify(TicketChild ticketChild, DataContext db, int organizationID, int ticketID, int id)
        {
            string query = String.Empty;
            switch(ticketChild)
            {
                case TicketChild.Action:
                    query = $"SELECT ActionID FROM Actions WITH (NOLOCK) WHERE ActionID={id} AND TicketID={ticketID}";
                    break;
                case TicketChild.Asset:
                    query = $"SELECT AssetID FROM Assets WITH (NOLOCK) WHERE AssetID={id} AND TicketID={ticketID}";
                    break;
                case TicketChild.TicketTagLinks:
                    query = $"SELECT TagLinkID FROM TagLinks WITH (NOLOCK) WHERE TagLinkID={id} AND RefID={ticketID} AND RefType=17";
                    break;
                case TicketChild.TicketReminders:
                    query = $"SELECT ReminderID FROM Reminders WITH (NOLOCK) WHERE ReminderID={id} AND OrganizationID={organizationID} AND RefID={ticketID} AND RefType=17";
                    break;
                case TicketChild.Subscriptions:
                    query = $"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
                            $"JOIN Users WITH (NOLOCK) ON users.userid = Subscriptions.userid " +
                            $"WHERE Subscriptions.userid = userID AND Reftype = 17 AND Refid = {ticketID} AND MarkDeleted = 0";
                    break;
                case TicketChild.TaskAssociations:
                    query = $"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE TaskID={id} AND Refid={ticketID} and RefType = 17";
                    break;
                case TicketChild.AssetTicket:
                    query = $"SELECT AssetID FROM AssetTickets WITH (NOLOCK) WHERE AssetID={id} AND TicketID={ticketID}";
                    break;
            }
            Verify(db, query);
        }

        private static void Verify(DataContext db, string query)
        {
            if (db.ExecuteQuery<int>(query).Any()) // valid ID found?
                return;

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(query);   // see the failed query in the debug output window
                Debugger.Break();   // ID is wrong - fix the code!
            }
            throw new System.Data.ConstraintException(String.Format($"{query} not found")); // error - a join of the records to authentication just doesn't add up
        }


        public static void VerifyActionAttachment(DataContext db, int organizationID, int ticketID, int actionID, int attachmentID)
        {
            Verify(db, $"SELECT AttachmentID FROM Attachments WITH (NOLOCK) WHERE ActionAttachmentID={attachmentID} AND OrganizationID={organizationID} AND RefID={actionID} AND RefType=0");
        }

        public static bool UserAllowUserToEditAnyAction(DataContext db, int userID)
        {
            return db.ExecuteQuery<bool>($"SELECT AllowUserToEditAnyAction FROM Users WITH (NOLOCK) WHERE UserID={userID}").First();
        }

        public static string AttachmentPath(DataContext db, int id)
        {
            return db.ExecuteQuery<string>($"SELECT[Value] FROM FilePaths WITH(NOLOCK) WHERE ID = {id}").FirstOrDefault();
        }

        public static int CreatorID(DataContext db, int actionID)
        {
            return db.ExecuteQuery<int>($"SELECT CreatorID FROM Actions WITH (NOLOCK) WHERE ActionID={actionID}").Min();
        }

        public static int AttachmentStorageUsed(DataContext db, int organizationID)
        {
            return db.ExecuteQuery<int>($"SELECT SUM(a.FileSize) FROM Attachments a WITH (NOLOCK) WHERE (a.OrganizationID = {organizationID}").Min();
        }
        public static int TicketNumber(DataContext db, int id)
        {
            return db.ExecuteQuery<int>($"SELECT TicketNumber FROM Tickets WITH(NOLOCK) WHERE TicketId = {id}").First();
        }


        public static int[] Read(TicketChild childID, TicketNode ticket)
        {
            string query = String.Empty;
            switch (childID)
            {
                case TicketChild.Contacts:
                    query = $"SELECT Users.userid FROM Users WITH (NOLOCK)" +
                        $"JOIN UserTickets WITH (NOLOCK) on UserTickets.userid = Users.UserID" +
                        $" WHERE UserTickets.TicketID = {ticket.TicketID} AND (Users.MarkDeleted = 0)";
                    break;
                case TicketChild.Customers:
                    query = $"Select Organizationid From OrganizationTickets WITH (NOLOCK) WHERE TicketId = {ticket.TicketID}";
                    break;
                case TicketChild.Subscriptions:
                    query = $"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
                        $"JOIN Users WITH (NOLOCK) on users.userid = Subscriptions.userid " +
                        $"WHERE Reftype = 17 and Refid = {ticket.TicketID} and MarkDeleted = 0";
                    break;
                case TicketChild.TicketReminders:
                    query = $"SELECT ReminderID FROM Reminders WITH (NOLOCK) WHERE RefID = {ticket.TicketID} AND Reftype = 17";
                    break;
                case TicketChild.TaskAssociations:
                    query = $"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE Refid={ticket.TicketID} and RefType = 17";
                    break;
                case TicketChild.Asset:
                    query = $"SELECT AssetID From AssetTickets WITH (NOLOCK) WHERE TicketID = {ticket.TicketID}";
                    break;
                case TicketChild.Children:
                    query = $"SELECT TicketID FROM Tickets WITH(NOLOCK) WHERE ParentID={ticket.TicketID}";
                    break;
                case TicketChild.TicketTagLinks:
                    query = $"SELECT TagLinkID FROM TagLinks WITH(NOLOCK) WHERE Reftype=17 and RefID = {ticket.TicketID}";
                    break;
            }
            return ticket.Request._db.ExecuteQuery<int>(query).ToArray();
        }

        public static int[] Read(TicketAssociation childID, TicketNode destinationTicket, TicketNode sourceTicket)
        {
            if (destinationTicket.Request != sourceTicket.Request)
                throw new Exception("tickets must come from the same connection");

            string query = String.Empty;
            switch (childID)
            {
                case TicketAssociation.QueueUsers:
                    query = $"SELECT TicketQueue.UserID " +
                        $"FROM TicketQueue WITH (NOLOCK) " +
                        $"JOIN Users WITH (NOLOCK) on Users.userid = TicketQueue.userid " +
                        $"LEFT JOIN TicketQueue TicketQueue2 WITH(NOLOCK) on TicketQueue2.userid = TicketQueue.userid and TicketQueue2.ticketid = {destinationTicket.TicketID} " +
                        $"WHERE TicketQueue.ticketid ={sourceTicket.TicketID}  and TicketQueue2.TicketQueueID IS NULL and MarkDeleted =0";
                    break;
                case TicketAssociation.Relationships1:
                    query = $"SELECT TicketRelationshipID FROM TicketRelationships WITH(NOLOCK) WHERE Ticket1ID={sourceTicket.TicketID} AND Ticket2ID <> {destinationTicket.TicketID}";
                    break;
                case TicketAssociation.Relationships2:
                    query = $"SELECT TicketRelationshipID FROM TicketRelationships WITH(NOLOCK) WHERE Ticket2ID={sourceTicket.TicketID} and Ticket1ID={destinationTicket.TicketID}";
                    break;
            }

            return sourceTicket.Request._db.ExecuteQuery<int>(query).ToArray();
        }

    }


}
