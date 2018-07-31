using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web;
using System.Diagnostics;

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
            Verify();
        }

        /// <summary> New action attachment with data from front end /// </summary>
        public ActionAttachment(ActionModel action, Data.LoginUser user, HttpPostedFile postedFile, HttpRequest request)
        {
            Action = action;
            _db = Action._db;

            File = new AttachmentFile(Action.AttachmentPath, postedFile);
            File.Save();
            ActionAttachmentID = AddAttachment(user, postedFile, request, Action.AttachmentPath, File.FilePath); // add ActionAttachment record
        }

        [Conditional("DEBUG")]
        void Verify()
        {
            string query = $"SELECT AttachmentID FROM ActionAttachments WITH (NOLOCK) WHERE AttachmentID={ActionAttachmentID} AND ActionID={Action.ActionID} AND OrganizationID={Action.Ticket.User.Organization.OrganizationID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        /// <summary> extracted from ts-app\TeamSupport.Handlers\UploadUtils.cs SaveFiles() </summary>
        private int AddAttachment(Data.LoginUser user, HttpPostedFile postedFile, HttpRequest request, string fileName, string filePath)
        {
            // insert ActionAttachment record
            Data.Attachment attachment = new Data.Attachments(user).AddNewAttachment();
            attachment.RefType = Data.ReferenceType.Actions;
            attachment.RefID = Action.ActionID;
            attachment.OrganizationID = Action.Ticket.User.Organization.OrganizationID;
            attachment.FileName = fileName;
            attachment.Path = filePath;
            attachment.FileType = postedFile.ContentType;
            attachment.FileSize = postedFile.ContentLength;
            attachment.FilePathID = 3;
            if (request.Form["description"] != null)
                attachment.Description = request.Form["description"].Replace("\n", "<br />");
            if (request.Form["productFamilyID"] != null && request.Form["productFamilyID"] != "-1")
                attachment.ProductFamilyID = Int32.Parse(request.Form["productFamilyID"]);
            attachment.Collection.Save();
            return attachment.AttachmentID;
        }

        public void Delete()
        {
            if (!Action.CanEdit())
                return;

            // set WITH (ROWLOCK) 
            Data.Attachment attachment = Data.Attachments.GetAttachment(Action.Ticket.User.Authentication.LoginUser, ActionAttachmentID.Value);
            attachment.DeleteFile();
            attachment.Delete();
            attachment.Collection.Save();
            ActionAttachmentID = null;
        }

    }
}

