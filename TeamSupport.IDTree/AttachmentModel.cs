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
        //IDNode AsIDNode { get; }    // back door to map class to IDNode at compile time
    }

    /// <summary>
    /// Base class for Attachments
    /// </summary>
    public class AttachmentModel : IDNode
    {
        public IAttachedTo AttachedTo { get; protected set; }  // what are we attached to?
        public int AttachmentID { get; protected set; }
        public AttachmentFile File { get; protected set; }

        public AttachmentModel(IAttachedTo attachedTo, int id) : base((attachedTo as IDNode).Connection)
        {
            AttachmentID = id;
            AttachedTo = attachedTo;
        }

        public override void Verify()
        {
            // also check if AttachedTo is valid?
            Verify($"SELECT AttachmentID FROM Attachments WITH (NOLOCK) WHERE AttachmentID={AttachmentID} AND OrganizationID={Connection.OrganizationID}");
        }

        public AttachmentModel(ConnectionContext connection, int id) : base(connection)
        {
        }

        //public string AttachmentPath { get { return AttachedTo.AttachmentPath; } }

    }

}

