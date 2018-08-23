using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;
using TeamSupport.Model;
using TeamSupport.Proxy;
using TeamSupport.DataAPI;

namespace TeamSupport.ModelAPI
{
    /// <summary> Raquel writes awesome code </summary>
    class TicketMerge
    {
        public ConnectionContext Connection { get; private set; }
        public TicketModel Source { get; private set; } // winning
        public TicketModel Destination { get; private set; }    // losing

        /// <summary> On a verified connection merge verified ticket to verified ticket </summary>
        public TicketMerge(ConnectionContext connection, TicketModel destination, TicketModel source)
        {
            Destination = destination;
            Source = source;
            Connection = connection;
        }

        public void Merge()
        {
            if (!Connection.User.CanEdit()) // sufficient permissions?
                return;

            //Get Different arrays for merge and only merge what is necessary
            int[] values;
            try
            {
                if (TicketAPI.TryReadAssets(Connection, Source, out values))
                    TicketAPI.MergeAssets(Connection, values, Source, Destination); //Assets

                if (TicketAPI.TryReadChildren(Connection, Source, out values))
                    TicketAPI.MergeChildren(Connection, values, Source, Destination);   //Children

                if (TicketAPI.TryReadContacts(Connection, Source, out values))
                    TicketAPI.MergeContacts(Connection, values, Source, Destination);   //Contacts   

                if (TicketAPI.TryReadCustomers(Connection, Source, out values))
                    TicketAPI.MergeCustomers(Connection, values, Source, Destination);  //Customers

                if (TicketAPI.TryReadReminders(Connection, Source, out values))
                    TicketAPI.MergeReminders(Connection, values, Source, Destination);  //Reminders

                if (TicketAPI.TryReadTags(Connection, Source, out values))
                    TicketAPI.MergeTags(Connection, values, Source, Destination);   //Tags

                if (TicketAPI.TryReadSubscriptions(Connection, Source, out values))
                    TicketAPI.MergeSubscriptions(Connection, values, Source, Destination);  //Subscriptions

                if (TicketAPI.TryReadTasks(Connection, Source, out values))
                    TicketAPI.MergeTasks(Connection, values, Source, Destination);  //Tags

                if (TicketAPI.TryReadRelationships1(Connection, Source, Destination, out values))
                    TicketAPI.MergeRelationships1(Connection, values, Source, Destination); //Relationships in first column of table 

                if (TicketAPI.TryReadRelationships2(Connection, Source, Destination, out values))
                    TicketAPI.MergeRelationships2(Connection, values, Source, Destination); //Relationships in second column of table

                if (TicketAPI.TryReadQueueUsers(Connection, Source, Destination, out values))
                    TicketAPI.MergeQueuedTickets(Connection, values, Source, Destination);  //Queue              

                //Actions
                TicketAPI.MergeActions(Connection, Source, Destination);

                //Remove Source Ticket
                TicketAPI.Delete(Connection, Source);

                //Modify Destination Ticket
                TicketAPI.Update(Connection, Destination);
            }

            catch
            {
                throw;
            }
        }
    }

}
