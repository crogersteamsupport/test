using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using TeamSupport.Data;
using TeamSupport.IDTree;


namespace TeamSupport.DataAPI
{
    [Obsolete ("use DataAPI CRUD")]
    public static class TicketAPI
    {

        /// <summary> Create Ticket </summary>
        public static void Create(ClientRequest connection, TicketProxy ticketProxy)
        {
            // TODO - create ticket
            DataAPI.LogMessage(ActionLogType.Insert, ReferenceType.Tickets, ticketProxy.TicketID, "Created Ticket");
        }

        /// <summary> Read Ticket </summary>
        public static TicketProxy Read(ClientRequest connection, TicketNode ticketModel)
        {
            Table<TicketProxy> table = connection._db.GetTable<TicketProxy>();
            return table.Where(t => t.TicketID == ticketModel.TicketID).First();
        }

        /// <summary> Update Ticket </summary>
        public static void Update(ClientRequest connection, TicketNode ticketModel)
        {
            // TODO - update ticket by Scot.. this one is just for ticket merge.
            string query = $"UPDATE Tickets WITH (ROWLOCK)" +
                $" SET DateModified = '{DateTime.UtcNow}', ModifierId = {connection.Authentication.UserID}";
                connection._db.ExecuteCommand(query);
            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, ticketModel.TicketID, "Updated Ticket");
        }

        /// <summary> Delete Ticket</summary>
        public static void Delete(ClientRequest connection, TicketNode ticketModel)
        {
            string query = $"DELETE FROM Tickets WHERE TicketId = {ticketModel.TicketID}";
            connection._db.ExecuteCommand(query);
            DataAPI.LogMessage(ActionLogType.Delete, ReferenceType.Tickets, ticketModel.TicketID, "Deleted Ticket");
        }

        #region MergeReads
        /// <summary> Read Ticket Contacts </summary>
        public static bool TryReadContacts(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"SELECT Users.userid FROM Users WITH (NOLOCK)" +
                $"JOIN UserTickets WITH (NOLOCK) on UserTickets.userid = Users.UserID" +
                $" WHERE UserTickets.TicketID = {ticket.TicketID} AND (Users.MarkDeleted = 0)";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read Ticket Customers </summary>        
        public static bool TryReadCustomers(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"Select organizationid From OrganizationTickets WITH (NOLOCK) Where TicketId = {ticket.TicketID}";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary> Read Ticket Tags
        /// For TicketMerge we only need the tagid from taglinks
        /// Tags Table has the tags, Taglinks has the relationship between tags and tickets.
        /// </summary>      
        public static bool TryReadTags(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"SELECT TagID FROM TagLinks WITH(NOLOCK) WHERE Reftype=17 and RefID = {ticket.TicketID}";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read Ticket Subscriptions</summary>        
        public static bool TryReadSubscriptions(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"SELECT Subscriptions.userid FROM Subscriptions WITH (NOLOCK) " +
                $"JOIN Users WITH (NOLOCK) on users.userid = Subscriptions.userid " +
                $"WHERE Reftype = 17 and Refid = {ticket.TicketID} and MarkDeleted = 0";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read quequed tickets</summary>
        public static bool TryReadQueueUsers(ClientRequest connection, TicketNode sourceTicket, TicketNode destinationTicket, out int[] values)
        {
            string query = $"SELECT TicketQueue.UserID " +
                $"FROM TicketQueue WITH (NOLOCK) " +
                $"JOIN Users WITH (NOLOCK) on Users.userid = TicketQueue.userid " +
                $"LEFT JOIN TicketQueue TicketQueue2 WITH(NOLOCK) on TicketQueue2.userid = TicketQueue.userid and TicketQueue2.ticketid = {destinationTicket.TicketID} " +
                $"WHERE TicketQueue.ticketid ={sourceTicket.TicketID}  and TicketQueue2.TicketQueueID IS NULL and MarkDeleted =0";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read Ticket Reminders</summary>
        public static bool TryReadReminders(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"SELECT ReminderID FROM Reminders WITH (NOLOCK) WHERE RefID = {ticket.TicketID} AND Reftype = 17";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read Ticket Tasks</summary>
        public static bool TryReadTasks(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"SELECT TaskID FROM TaskAssociations WITH (NOLOCK) WHERE Refid={ticket.TicketID} and RefType = 17";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read Ticket Assets</summary>
        public static bool TryReadAssets(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"SELECT AssetID From AssetTickets WITH (NOLOCK) WHERE TicketID = {ticket.TicketID}";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read releated tickets 1 </summary>     
        public static bool TryReadRelationships1(ClientRequest connection, TicketNode sourceTicket, TicketNode destinationTicket, out int[] values)
        {
            string query = $"SELECT TicketRelationshipID FROM TicketRelationships WITH(NOLOCK) WHERE Ticket1ID={sourceTicket.TicketID} AND Ticket2ID <> {destinationTicket.TicketID}";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read releated tickets 2 </summary>     
        public static bool TryReadRelationships2(ClientRequest connection, TicketNode sourceTicket, TicketNode destinationTicket, out int[] values)
        {
            string query = $"SELECT TicketRelationshipID FROM TicketRelationships WITH(NOLOCK) WHERE Ticket2ID={sourceTicket.TicketID} and Ticket1ID={destinationTicket.TicketID}";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }

        /// <summary>Read children of ticket</summary> 
        public static bool TryReadChildren(ClientRequest connection, TicketNode ticket, out int[] values)
        {
            string query = $"SELECT TicketID FROM Tickets WITH(NOLOCK) WHERE ParentID={ticket.TicketID}";
            values = connection._db.ExecuteQuery<int>(query).ToArray();
            return values.Length > 0;
        }
        #endregion
        #region Merge updates

        public static void MergeAssets(ClientRequest connection, int[] assets, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            string query = "";
            foreach (int asset in assets)
            {
                query = $"UPDATE AssetTickets WITH(ROWLOCK) SET TicketID={destinationTicket.TicketID} WHERE AssetId = {asset} AND TicketId ={sourceTicket.TicketID}" +
                   $" AND NOT EXISTS (Select AssetTickets WITH (NOLOCK) WHERE AssetId = {asset} AND TicketId ={sourceTicket.TicketID})";
                connection._db.ExecuteCommand(query);
            }
            //cleanup after inserts?
            query = $"DELETE FROM AssetTickets WITH (ROWLOCK)  WHERE TicketID ={sourceTicket.TicketID} ";
            connection._db.ExecuteCommand(query);

            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Assets");
        }

        public static void MergeChildren(ClientRequest connection, int[] children, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            foreach (int child in children)
            {
                //TODO: Think about when parentid = ticketid
                string query = $" UPDATE Tickets WITH(ROWLOCK) SET ParentID ={destinationTicket.TicketID} WHERE(ParentID = {child})";
                connection._db.ExecuteCommand(query);
            }
            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' children tickets");
        }

        public static void MergeContacts(ClientRequest connection, int[] contacts, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            foreach (int contact in contacts)
            {
                string query = $"DELETE FROM UserTickets Where TicketID={sourceTicket.TicketID} AND UserId = {contact}";
                connection._db.ExecuteCommand(query);
                query = $"INSERT INTO UserTickets (TicketID, UserID, DateCreated, CreatorID)" +
                        $"SELECT {destinationTicket.TicketID}, {contact}, '{DateTime.UtcNow}', {connection.Authentication.UserID} " +
                        $"WHERE NOT EXISTS(SELECT * FROM UserTickets WHERE TicketID ={destinationTicket.TicketID} and UserID ={contact})";
                connection._db.ExecuteCommand(query);
            }

            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Contacts");
        }

        public static void MergeCustomers(ClientRequest connection, int[] customers, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            foreach (int customer in customers)
            {
                string query = $"DELETE FROM OrganizationTickets Where TicketID={sourceTicket.TicketID} AND OrganizationId = {customer}";
                connection._db.ExecuteCommand(query);
                query = $"INSERT INTO OrganizationTickets (TicketID, OrganizationID, DateCreated, CreatorID, DateModified, ModifierID)" +
                        $"SELECT {destinationTicket.TicketID}, {customer}, '{DateTime.UtcNow}', {connection.Authentication.UserID}, '{DateTime.UtcNow}', {connection.Authentication.UserID}" +
                        $"WHERE NOT EXISTS(SELECT * FROM OrganizationTickets WHERE TicketID ={destinationTicket.TicketID} and OrganizationId ={customer})";
                connection._db.ExecuteCommand(query);
            }
            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Customers");
        }

        public static void MergeQueuedTickets(ClientRequest connection, int[] queued, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            string query;
            foreach (int queue in queued)
            {
                query = $"SELECT MAX(Position) FROM TicketQueue WHERE UserID = {queue}";
                int position = connection._db.ExecuteQuery<int>(query).FirstOrDefault();

                query = $"Update TicketQueue WITH(ROWLOCK) SET TicketID = {destinationTicket.TicketID}, Position = {position + 1} WHERE Userid= {queue}"; ;
                connection._db.ExecuteCommand(query);
            }
            //cleanup after inserts?
            query = $"DELETE FROM TicketQueue WITH (ROWLOCK)  WHERE TicketID ={sourceTicket.TicketID} ";
            connection._db.ExecuteCommand(query);

            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Queued Tickets");
        }


        /// <summary>
        /// Update reminders with destination ticket. Duplicates are allowed in table.
        /// </summary>

        public static void MergeReminders(ClientRequest connection, int[] reminders, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            foreach (int reminder in reminders)
            {
                string query = $" UPDATE Reminders WITH(ROWLOCK) SET RefID ={destinationTicket.TicketID} WHERE(ReminderId = {reminder})";
                connection._db.ExecuteCommand(query);
            }
            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Reminders");
        }

        public static void MergeTasks(ClientRequest connection, int[] tasks, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            foreach (int task in tasks)
            {
                string query = $" UPDATE TaskAssociations WITH(ROWLOCK) SET RefID ={destinationTicket.TicketID} WHERE(TaskId = {task})";
                connection._db.ExecuteCommand(query);
            }
            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Tasks");
        }


        public static void MergeTags(ClientRequest connection, int[] tags, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            string query;
            foreach (int tag in tags)
            {
                query = $" UPDATE TagLinks WITH(ROWLOCK) SET RefID ={destinationTicket.TicketID} WHERE(TagID = {tag} AND RefID = {sourceTicket.TicketID} AND reftype = 17)" +
                   $"AND NOT EXISTS (SELECT * FROM TagLinks WITH (NOLOCK) WHERE RefID={destinationTicket.TicketID} and TagID = {tag} and Refype = 17)  ";
                connection._db.ExecuteCommand(query);
            }

            //Cleanup any tags left 
            query = $"DELETE FROM TagLinks WITH (ROWLOCK) WHERE RefID={sourceTicket.TicketID} and Refype = 17";
            connection._db.ExecuteCommand(query);

            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Tags");
        }

        public static void MergeSubscriptions(ClientRequest connection, int[] subscriptions, TicketNode sourceTicket, TicketNode destinationTicket)

        {
            foreach (int subscription in subscriptions)
            {
                string query = $"DELETE FROM Subscriptions Where RefId={sourceTicket.TicketID} AND RefType = 17 and UserId = {subscription}";
                connection._db.ExecuteCommand(query);
                query = $"INSERT INTO Subscriptions (RefType, RefID, UserID, DateCreated, DateModified, CreatorID, ModifierID)" +
                        $"SELECT 17, {destinationTicket.TicketID},{subscription}, '{DateTime.UtcNow}','{DateTime.UtcNow}', {connection.Authentication.UserID},  {connection.Authentication.UserID} " +
                        $"WHERE NOT EXISTS(SELECT * FROM Subscriptions WHERE reftype = 17 AND RefID = {destinationTicket.TicketID} AND UserID = {subscription})";
                connection._db.ExecuteCommand(query);
            }
            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Subscriptions");
        }
        public static void MergeRelationships1(ClientRequest connection, int[] relationships, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            string query;
            foreach (int relationship in relationships)
            {
                query = $" UPDATE TicketRelationships WITH(ROWLOCK) SET Ticket1ID ={destinationTicket.TicketID} WHERE TicketRelationshipID={relationship}";
                connection._db.ExecuteCommand(query);
            }

            //Cleanup any ticketrleationships left
            query = $"DELETE FROM TicketRelationships WITH (ROWLOCK) WHERE Ticket1ID={sourceTicket.TicketID}";
            connection._db.ExecuteCommand(query);

            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Related Tickets");
        }

        public static void MergeRelationships2(ClientRequest connection, int[] relationships, TicketNode sourceTicket, TicketNode destinationTicket)
        {
            string query;
            foreach (int relationship in relationships)
            {
                query = $" UPDATE TicketRelationships WITH(ROWLOCK) SET Ticket2ID ={destinationTicket.TicketID} WHERE TicketRelationshipID={relationship}";
                connection._db.ExecuteCommand(query);
            }

            //Cleanup any ticketrleationships left
            query = $"DELETE FROM TicketRelationships WITH (ROWLOCK) WHERE Ticket2ID={sourceTicket.TicketID}";
            connection._db.ExecuteCommand(query);

            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Related Tickets");
        }

        public static void MergeActions(ClientRequest connection, TicketNode sourceTicket, TicketNode destinationTicket)
        {

            string query = $"SELECT actionId from Actions with (NOLOCK) where TicketId = {sourceTicket.TicketID}";
            int[] actions = connection._db.ExecuteQuery<int>(query).ToArray();
            foreach (int action in actions)
            {
                query = $" UPDATE Actions WITH(ROWLOCK) SET TicketID ={destinationTicket.TicketID} WHERE(ActionId = {action})";
                connection._db.ExecuteCommand(query);
            }
            DataAPI.LogMessage(ActionLogType.Update, ReferenceType.Tickets, destinationTicket.TicketID, "Merged '" + sourceTicket.TicketNumber + "' Actions");
        }


    }
    #endregion


}


