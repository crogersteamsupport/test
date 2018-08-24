using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamSupport.Data;
using TeamSupport.IDTree;
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

        /// <summary> Merge1 </summary>
        public void Merge1()
        {
            if (!Connection.User.CanEdit()) // sufficient permissions?
                return;

            MergeAssets();
            //MergeChildren();
            MergeContacts();
            MergeCustomers();
            MergeReminders();
            MergeTagLinks();
            MergeSubscriptions();
            MergeTaskAssociations();
            //MergeRelationships()  // 1 and 2
            //MergeQueueUsers();
            //MergeActions();

            //DataAPI.DataAPI.Delete(Source);
            //Source = null;
            //DataAPI.DataAPI.Update(Destination, DataAPI.DataAPI.Read<TicketProxy>(Destination));  // update Date Modified
        }

        void MergeAssets()
        {
            AssetTicketModel[] assetTickets = AssetTicketModel.GetAssetTickets(Source);
            if (assetTickets.Length == 0)
                return;

            AssetTicketModel[] destinationAssetTickets = AssetTicketModel.GetAssetTickets(Destination);
            foreach (AssetTicketModel assetTicket in assetTickets)
            {
                // WHERE NOT EXISTS (Select AssetTickets WITH (NOLOCK) WHERE AssetId = {asset} AND TicketId ={sourceTicket.TicketID})
                if (!destinationAssetTickets.Where(a => a.AssetID == assetTicket.AssetID).Any())
                {
                    AssetTicketProxy assetTicketProxy = new AssetTicketProxy() { AssetID = assetTicket.AssetID };
                    DataAPI.DataAPI.Update(assetTicket, assetTicketProxy);
                    continue;
                }

                 DataAPI.DataAPI.Delete(assetTicket);
            }
        }

        void MergeCustomers()
        {
            Customer[] customers = Customer.GetCustomers(Source);
            if (customers.Length == 0)
                return;

            Customer[] destinationCustomers = Customer.GetCustomers(Destination);
            foreach (Customer customer in customers)
            {
                // WHERE NOT EXISTS(SELECT * FROM OrganizationTickets WHERE TicketID ={model.TicketID} and OrganizationId ={proxy.OrganizationID})
                if (!destinationCustomers.Where(c => c.OrganizationID == customer.OrganizationID).Any())
                {
                    CustomerProxy customerProxy = new CustomerProxy() { OrganizationID = customer.OrganizationID };
                    DataAPI.DataAPI.Create(Destination, customerProxy);
                }

                DataAPI.DataAPI.Delete(customer);
            }
        }

        void MergeContacts()
        {
            Contact[] contacts = Contact.GetContacts(Source);
            if (contacts.Length == 0)
                return;

            Contact[] destinationContacts = Contact.GetContacts(Destination);
            foreach (Contact contact in contacts)
            {
                // WHERE NOT EXISTS(SELECT * FROM UserTickets WHERE TicketID ={model.TicketID} and UserID ={proxy.UserID})
                if (!destinationContacts.Where(c => c.UserID == contact.UserID).Any())
                {
                    ContactProxy contactProxy = new ContactProxy() { UserID = contact.UserID };
                    DataAPI.DataAPI.Create(Destination, contactProxy);
                }

                DataAPI.DataAPI.Delete(contact);
            }
        }

        /// <summary> TagLinks </summary>
        void MergeTagLinks()
        {
            TagLinkProxy[] sourceTagLinks = DataAPI.DataAPI.Read<TagLinkProxy[]>(Source);
            if (sourceTagLinks.Length == 0)
                return;

            TagLinkProxy[] destinationTagLinks = DataAPI.DataAPI.Read<TagLinkProxy[]>(Destination);
            foreach (TagLinkProxy tagLinkProxy in sourceTagLinks)
            {
                // WHERE NOT EXISTS(SELECT* FROM TagLinks WITH (NOLOCK) WHERE RefID={destinationTicket.TicketID} AND TagID = {tag} AND Refype = 17)
                if (!destinationTagLinks.Where(t => t.TagID == tagLinkProxy.TagID).Any())
                {
                    tagLinkProxy.RefID = Destination.TicketID;
                    DataAPI.DataAPI.Update(new TagLinkModel(Source, tagLinkProxy.TagLinkID), tagLinkProxy);
                    continue;
                }

                DataAPI.DataAPI.Delete(new TagLinkModel(Source, tagLinkProxy.TagLinkID));
            }
        }

        /// <summary> Subscriptions </summary>
        void MergeSubscriptions()
        {
            SubscriptionModel[] subscriptions = SubscriptionModel.GetSubscriptions(Source);
            if (subscriptions.Length == 0)
                return;

            SubscriptionModel[] destinationSubscriptions = SubscriptionModel.GetSubscriptions(Destination);
            foreach (SubscriptionModel subscriptionModel in subscriptions)
            {
                // WHERE NOT EXISTS(SELECT * FROM Subscriptions WHERE reftype = 17 AND RefID = {model.TicketID} AND UserID = {proxy.UserID})
                if (!destinationSubscriptions.Where(s => s.UserID == subscriptionModel.UserID).Any())
                {
                    SubscriptionProxy subscriptionProxy = new SubscriptionProxy()
                    {
                        RefType = ReferenceType.Tickets,
                        RefID = Destination.TicketID,
                        UserID = subscriptionModel.UserID
                    };
                    DataAPI.DataAPI.Create(Destination, subscriptionProxy);
                }

                DataAPI.DataAPI.Delete(subscriptionModel);
            }
        }

        void MergeReminders()
        {
            ReminderModel[] reminders = ReminderModel.GetReminders(Source);
            foreach (ReminderModel reminderModel in reminders)
            {
                ReminderProxy reminderProxy = DataAPI.DataAPI.Read<ReminderProxy>(reminderModel);
                DataAPI.DataAPI.Update(Destination, reminderProxy);
            }
        }

        void MergeTaskAssociations()
        {
            TaskAssociationModel[] tasks = TaskAssociationModel.GetTaskAssociations(Source);
            foreach (TaskAssociationModel task in tasks)
            {
                TaskAssociationProxy taskAssociationProxy = DataAPI.DataAPI.Read<TaskAssociationProxy>(task);
                DataAPI.DataAPI.Update(Destination, taskAssociationProxy);
            }
        }

        public void Merge()
        {
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
