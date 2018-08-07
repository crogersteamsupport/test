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

namespace TeamSupport.Model
{
    /// <summary> Action Attachments </summary>
    public class ActionAttachment
    {
        public ActionModel Action { get; private set; }
        public int? ActionAttachmentID { get; private set; }
        public DataContext _db { get; private set; }
        public AttachmentFile File { get; private set; }

        /// <summary> Load existing action attachment /// </summary>
        public ActionAttachment(ActionModel action, int actionAttachmentID)
        {
            Action = action;
            ActionAttachmentID = actionAttachmentID;
            _db = action._db;

            TicketModel ticket = Action.Ticket;
            OrganizationModel organization = ticket.User.Organization;
            DBReader.VerifyActionAttachment(_db, organization.OrganizationID, ticket.TicketID, Action.ActionID, ActionAttachmentID.Value);
        }

        /// <summary> New action attachment with data from front end /// </summary>
        public ActionAttachment(ActionModel action, AuthenticationModel authentication, HttpPostedFile postedFile, HttpRequest request)
        {
            Action = action;
            _db = Action._db;

            File = new AttachmentFile(Action.AttachmentPath, postedFile);
            File.Save();
            AttachmentProxy proxy = InsertActionAttachment(authentication, postedFile, request, File.FileName, File.FilePath); // add ActionAttachment record
            ActionAttachmentID = proxy.AttachmentID;
        }

        /// <summary> extracted from ts-app\TeamSupport.Handlers\UploadUtils.cs SaveFiles() </summary>
        private AttachmentProxy InsertActionAttachment(AuthenticationModel authentication, HttpPostedFile postedFile, HttpRequest request, string fileName, string filePath)
        {
            string description = request.Form["description"];
            if (description != null)
                description = description.Replace("\n", "<br />");

            int? productFamilyID = null;
            string tmp = request.Form["productFamilyID"];
            if ((tmp != null) && !tmp.Equals("-1"))
                productFamilyID = Int32.Parse(tmp);

            DateTime now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            AttachmentProxy proxy = new AttachmentProxy()
            {
                FilePathID = 3,
                //SentToSnow = ,
                //SentToTFS = ,
                ProductFamilyID = productFamilyID,
                //SentToJira = ,
                RefID = Action.ActionID,
                //RefType = Data.ReferenceType.Actions,
                ModifierID = authentication.UserID,
                CreatorID = authentication.UserID,
                Description = description,
                Path = filePath,
                FileSize = postedFile.ContentLength,
                FileType = postedFile.ContentType,
                FileName = fileName,
                OrganizationID = Action.Ticket.User.Organization.OrganizationID,
                //AttachmentID = this.AttachmentID,
                //CreatorName = Action.Ticket.User.CreatorName(),
                DateCreated = now,
                DateModified = now
            };

            // insert into DB and get back ActionAttachmentID
            //DataAPI.DataAPI.InsertActionAttachment(_db, Action.Ticket.TicketID, ref proxy);
            return proxy;
        }

        public void Delete()
        {
            if (!Action.CanEdit())
                return;

            TicketModel ticket = Action.Ticket;
            UserSession user = ticket.User;
            OrganizationModel organization = user.Organization;
            //DataAPI.DataAPI.DeleteActionAttachment(user.Authentication, organization.OrganizationID, ticket.TicketID, Action.ActionID, ActionAttachmentID.Value);
            ActionAttachmentID = null;
        }

    }
}

