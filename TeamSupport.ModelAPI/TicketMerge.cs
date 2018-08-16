using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;
using TeamSupport.Model;
using TeamSupport.Proxy;

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
                if(DataAPI.TicketAPI.TryReadAssets(Connection, Source, out values))                           
                    DataAPI.TicketAPI.MergeAssets(Connection, values, Source, Destination);            
                if (DataAPI.TicketAPI.TryReadChildren(Connection, Source, out values))            
                    DataAPI.TicketAPI.MergeChildren(Connection, values, Source, Destination);            
                if (DataAPI.TicketAPI.TryReadContacts(Connection, Source, out values))            
                    DataAPI.TicketAPI.MergeContacts(Connection, values, Source, Destination);            
                if (DataAPI.TicketAPI.TryReadCustomers(Connection, Source, out values))
                    DataAPI.TicketAPI.MergeCustomers(Connection, values, Source, Destination);
                if (DataAPI.TicketAPI.TryReadReminders(Connection, Source, out values))
                    DataAPI.TicketAPI.MergeReminders(Connection, values, Source, Destination);
                if (DataAPI.TicketAPI.TryReadTags(Connection, Source, out values))
                    DataAPI.TicketAPI.MergeTags(Connection, values, Source, Destination);
                if (DataAPI.TicketAPI.TryReadSubscriptions(Connection, Source, out values))
                    DataAPI.TicketAPI.MergeSubscriptions(Connection, values, Source, Destination);
                if (DataAPI.TicketAPI.TryReadTasks(Connection, Source, out values))
                    DataAPI.TicketAPI.MergeTasks(Connection, values, Source, Destination);

                //Do with proxies
                if (DataAPI.TicketAPI.TryReadQueueUsers(Connection, Source, out values))
                {

                }
                if (DataAPI.TicketAPI.TryReadRelationships1(Connection, Source, out values))
                {

                }
                if (DataAPI.TicketAPI.TryReadRelationships2(Connection, Source, out values))
                {

                }


                //Actions
                DataAPI.TicketAPI.MergeActions(Connection, Source, Destination);

                //Remove Source Ticket
                DataAPI.TicketAPI.Delete(Connection, Source);
                
                //Modify Destination Ticket

               //Connection.Commit();
                }


            catch
            {               
                throw;
            }
        }
    }

}
