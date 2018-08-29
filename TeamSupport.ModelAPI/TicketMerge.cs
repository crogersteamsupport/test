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
            if (!Connection.CanEdit()) // sufficient permissions?
                return;

            MergeAssets();
            MergeChildren();
            MergeContacts();
            MergeCustomers();
            MergeReminders();
            MergeTagLinks();
            MergeSubscriptions();
            MergeTaskAssociations();
            MergeRelationships();  // 1 and 2
            //MergeQueuedTickets();
            MergeActions();

            DataAPI.DataAPI.Delete(Source);
            //Source = null;
            //DataAPI.DataAPI.Update(Destination, null);  // update Date Modified
        }

        public void MergeActions()
        {
            ActionModel[] actions = Source.Actions();
            foreach (ActionModel action in actions)
                DataAPI.DataAPI.Update(action, new UpdateArguments("TicketID", Destination.TicketID));
        }

        enum TicketRelationship
        {
            SourceIsDestination,
            SourceTicket2,
            Ticket1Source
        }

        TicketRelationship GetState(TicketRelationshipProxy proxy)
        {
            if (proxy.Ticket1ID == Source.TicketID)
                return proxy.Ticket2ID == Destination.TicketID ? TicketRelationship.SourceIsDestination : TicketRelationship.SourceTicket2;

            return proxy.Ticket1ID == Destination.TicketID ? TicketRelationship.SourceIsDestination : TicketRelationship.Ticket1Source;
        }

        void MergeRelationships()
        {
            TicketRelationshipProxy[] proxies = DataAPI.DataAPI.Read<TicketRelationshipProxy[]>(Source);
            TicketRelationshipModel idNode;
            foreach (TicketRelationshipProxy proxy in proxies)
            {
                switch(GetState(proxy))
                {
                    case TicketRelationship.SourceIsDestination:
                        idNode = new TicketRelationshipModel(Source, Destination, proxy.TicketRelationshipID);
                        DataAPI.DataAPI.Delete(idNode);   // circular reference
                        break;
                    case TicketRelationship.SourceTicket2:
                        TicketModel ticket2 = new TicketModel(Source.Connection, proxy.Ticket2ID);
                        idNode = new TicketRelationshipModel(Source, ticket2, proxy.TicketRelationshipID);
                        DataAPI.DataAPI.Update(idNode, new UpdateArguments("Ticket1ID", Destination.TicketID));
                        break;
                    case TicketRelationship.Ticket1Source:
                        TicketModel ticket1 = new TicketModel(Source.Connection, proxy.Ticket1ID);
                        idNode = new TicketRelationshipModel(ticket1, Source, proxy.TicketRelationshipID);
                        DataAPI.DataAPI.Update(idNode, new UpdateArguments("Ticket2ID", Destination.TicketID));
                        break;
                }
            }
        }

        void MergeChildren()
        {
            //TODO: Think about when parentid = ticketid
            UpdateArguments args = new UpdateArguments("ParentID", Destination.TicketID);
            TicketModel[] children = Source.ChildTickets();
            foreach (TicketModel child in children)
                DataAPI.DataAPI.Update(child, args);
        }

        void MergeAssets()
        {
            AssetTicketModel[] assetTickets = Source.AssetTickets();
            if (assetTickets.Length == 0)
                return;

            AssetTicketModel[] destinationAssetTickets = Destination.AssetTickets();
            UpdateArguments args = new UpdateArguments("TicketID", Destination.TicketID);
            foreach (AssetTicketModel assetTicket in assetTickets)
            {
                // WHERE NOT EXISTS (Select AssetTickets WITH (NOLOCK) WHERE AssetId = {asset} AND TicketId ={sourceTicket.TicketID})
                if (!destinationAssetTickets.Where(a => a.Asset.AssetID == assetTicket.Asset.AssetID).Any())
                    DataAPI.DataAPI.Update(assetTicket, args);
                else
                    DataAPI.DataAPI.Delete(assetTicket);
            }
        }

        void MergeCustomers()
        {
            OrganizationTicketModel[] customers = Source.OrganizationTickets();
            if (customers.Length == 0)
                return;

            OrganizationTicketModel[] destinationCustomers = Destination.OrganizationTickets();
            foreach (OrganizationTicketModel customer in customers)
            {
                // WHERE NOT EXISTS(SELECT * FROM OrganizationTickets WHERE TicketID ={model.TicketID} and OrganizationId ={proxy.OrganizationID})
                if (!destinationCustomers.Where(c => c.Organization.OrganizationID == customer.Organization.OrganizationID).Any())
                {
                    CustomerProxy customerProxy = new CustomerProxy() { OrganizationID = customer.Organization.OrganizationID };
                    DataAPI.DataAPI.Create(Destination, customerProxy);
                }

                DataAPI.DataAPI.Delete(customer);
            }
        }

        void MergeContacts()
        {
            UserTicketModel[] contacts = Source.UserTickets();
            if (contacts.Length == 0)
                return;

            UserTicketModel[] destinationContacts = Destination.UserTickets();
            foreach (UserTicketModel contact in contacts)
            {
                // WHERE NOT EXISTS(SELECT * FROM UserTickets WHERE TicketID ={model.TicketID} and UserID ={proxy.UserID})
                if (!destinationContacts.Where(c => c.User.UserID == contact.User.UserID).Any())
                {
                    ContactProxy contactProxy = new ContactProxy() { UserID = contact.User.UserID };
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
                //// WHERE NOT EXISTS(SELECT* FROM TagLinks WITH (NOLOCK) WHERE RefID={destinationTicket.TicketID} AND TagID = {tag} AND Refype = 17)
                //if (!destinationTagLinks.Where(t => t.TagID == tagLinkProxy.TagID).Any())
                //{
                //    tagLinkProxy.RefID = Destination.TicketID;
                //    TagNode tag = new TagNode(Source.Organization, );
                //    DataAPI.DataAPI.Update(new TagLinkNode(Source, tag), tagLinkProxy);
                //    continue;
                //}

                //DataAPI.DataAPI.Delete(new TagLinkNode(Source, tagLinkProxy.TagLinkID));
            }
        }

        /// <summary> Subscriptions </summary>
        void MergeSubscriptions()
        {
            SubscriptionModel[] subscriptions = Source.Subscriptions();
            if (subscriptions.Length == 0)
                return;

            SubscriptionModel[] destinationSubscriptions = Destination.Subscriptions();
            foreach (SubscriptionModel subscriptionModel in subscriptions)
            {
                // WHERE NOT EXISTS(SELECT * FROM Subscriptions WHERE reftype = 17 AND RefID = {model.TicketID} AND UserID = {proxy.UserID})
                if (!destinationSubscriptions.Where(s => s.User.UserID == subscriptionModel.User.UserID).Any())
                {
                    SubscriptionProxy subscriptionProxy = new SubscriptionProxy()
                    {
                        RefType = ReferenceType.Tickets,
                        RefID = Destination.TicketID,
                        UserID = subscriptionModel.User.UserID
                    };
                    DataAPI.DataAPI.Create(Destination, subscriptionProxy);
                }

                DataAPI.DataAPI.Delete(subscriptionModel);
            }
        }

        void MergeReminders()
        {
            TicketReminderModel[] reminders = Source.Reminders();
            UpdateArguments args = new UpdateArguments("RefID", Destination.TicketID);
            foreach (TicketReminderModel reminderModel in reminders)
                DataAPI.DataAPI.Update(Destination, args);
        }

        void MergeTaskAssociations()
        {
            TaskAssociationModel[] taskAssociations = Source.TaskAssociations();
            UpdateArguments args = new UpdateArguments("RefID", Destination.TicketID);
            foreach (TaskAssociationModel task in taskAssociations)
                DataAPI.DataAPI.Update(Destination, args);
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
