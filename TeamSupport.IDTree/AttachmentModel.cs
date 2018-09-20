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

namespace TeamSupport.IDTree
{
    // interface to model class that supports attachments 
    public interface IAttachedTo
    {
        string AttachmentPath { get; }
        IDNode AsIDNode { get; }    // back door to map class to IDNode at compile time
        //int ID { get; }
    }

    public class AttachmentModelT : IDNode
    {
        public IAttachedTo AttachedTo { get; protected set; }  // Action, Contact, Asset, Task... (from RefType)
        public int AttachmentID { get; protected set; }
        public AttachmentFile File { get; protected set; }

        public AttachmentModelT(IAttachedTo attachedTo, int attachmentID) : base(attachedTo.AsIDNode.Connection)
        {
            AttachedTo = attachedTo;
            AttachmentID = attachmentID;
        }

        public string AttachmentPath { get { return AttachedTo.AttachmentPath; } }
        public override void Verify()
        {
            Verify($"SELECT AttachmentID FROM Attachments WITH (NOLOCK) WHERE AttachmentID={AttachmentID} AND OrganizationID={Connection.OrganizationID}");
        }
        protected static int AttachmentParentID(ConnectionContext connection, int attachmentID)
        {
            return connection._db.ExecuteQuery<int>($"SELECT RefID FROM Attachments WITH(NOLOCK) WHERE AttachmentID = {attachmentID} AND OrganizationID={connection.OrganizationID}").Min();
        }
    }

    /// <summary>
    /// Base class for Attachments
    /// </summary>
    public abstract class AttachmentModel : IDNode
    {
        public IAttachedTo AttachedTo { get; protected set; }  // what are we attached to?
        public int AttachmentID { get; protected set; }
        public AttachmentFile File { get; protected set; }

        public AttachmentModel(IAttachedTo attachedTo, int id) : this(attachedTo.AsIDNode.Connection, id)
        {
            AttachmentID = id;
            AttachedTo = attachedTo;
        }

        public AttachmentModel(ConnectionContext connection, int id) : base(connection)
        {
        }
    }


    /// <summary> Action Attachments</summary>
    public class ActionAttachmentModel : AttachmentModel
    {
        public ActionModel Action { get; private set; }
        public string AttachmentPath { get { return Action.AttachmentPath; } }


        /// <summary> top down - Load existing action attachment /// </summary>
        public ActionAttachmentModel(ActionModel action, int actionAttachmentID) : base(action, actionAttachmentID)
        {
            Action = action;
            AttachmentID = actionAttachmentID;
            Verify();
        }

        /// <summary> bottom up - Load existing action attachment /// </summary>
        public ActionAttachmentModel(ConnectionContext connection, int actionAttachmentID) : base(connection, actionAttachmentID)
        {
            AttachmentID = actionAttachmentID;
            int actionID = GetActionID(connection._db, AttachmentID);
            Action = new ActionModel(Connection, actionID);
            Verify();
        }

        public override void Verify()
        {
            TicketModel ticket = Action.Ticket;
            OrganizationModel organization = ticket.Organization;
            Verify($"SELECT AttachmentID FROM Attachments WITH (NOLOCK) " +
                $"WHERE AttachmentID={AttachmentID} AND OrganizationID={organization.OrganizationID} AND RefID={Action.ActionID} AND RefType=0");
        }
        public static int GetActionID(DataContext db, int attachmentID)
        {
            return db.ExecuteQuery<int>($"SELECT RefID FROM Attachments WITH(NOLOCK) WHERE AttachmentID = {attachmentID}").Min();
        }

    }

    /// <summary> Task Attachments</summary>
    public class TaskAttachmentModel : AttachmentModel
    {
        public TaskModel Task { get; private set; }
        public string AttachmentPath { get { return Task.AttachmentPath; } }

        public TaskAttachmentModel(TaskModel model, int attachmentID) : base(model, attachmentID)
        {
            Task = model;
        }

        public override void Verify()
        {
        }

    }

    public class AssetAttachmentModel : AttachmentModel
    {
        public AssetModel Asset { get; private set; }
        public string AttachmentPath { get { return Asset.AttachmentPath; } }

        public AssetAttachmentModel(AssetModel model, int attachmentID) : base(model, attachmentID)
        {
            Asset = model;
        }

        public override void Verify()
        {
        }
    }




}

