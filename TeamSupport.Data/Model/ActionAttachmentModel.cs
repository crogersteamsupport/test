using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web;
using System.Diagnostics;

namespace TeamSupport.Data.Model
{
    /// <summary>
    /// see 
    /// </summary>
    public class ActionAttachmentModel
    {
        public ActionModel Action { get; private set; }
        public int ActionAttachmentID { get; private set; }
        public DataContext _db { get; private set; }

        public string FileName { get; private set; }
        public string AttachmentPath { get; private set; }
        public string ContentType { get; private set; }
        public int ContentLength { get; private set; }

        public ActionAttachmentModel(ActionModel action, LoginUser user, HttpPostedFile postedFile, HttpRequest request)
        {
            Action = action;
            _db = Action._db;
            ContentType = postedFile.ContentType;
            ContentLength = postedFile.ContentLength;

            // save file
            FileName = ValidateFileName(postedFile.FileName);
            string filePath = Path.Combine(Action.AttachmentPath, FileName);
            postedFile.SaveAs(filePath);

            Attachment attachment = AddAttachment(user, postedFile, request, AttachmentPath, filePath);
            ActionAttachmentID = attachment.AttachmentID;
            Validate();
        }

        /// <summary>
        /// OLD DATA LAYER - extracted from ts-app\TeamSupport.Handlers\UploadUtils.cs SaveFiles()
        /// </summary>
        private Attachment AddAttachment(LoginUser user, HttpPostedFile postedFile, HttpRequest request, string fileName, string filePath)
        {
            // insert ActionAttachment record
            Attachment attachment = new Attachments(user).AddNewAttachment();
            attachment.RefType = ReferenceType.Actions;
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
            return attachment;
        }

        [Conditional("DEBUG")]
        void Validate()
        {
            string query = $"SELECT AttachmentID FROM ActionAttachments WITH (NOLOCK) WHERE AttachmentID={ActionAttachmentID} AND ActionID={Action.ActionID} AND OrganizationID={Action.Ticket.User.Organization.OrganizationID}";
            //string query = $"SELECT AttachmentID FROM ActionAttachments WITH (NOLOCK) WHERE AttachmentID={ActionAttachmentID} AND ActionID={Action.ActionID} AND TicketID={Action.Ticket.TicketID} AND OrganizationID={Action.Ticket.User.Organization.OrganizationID}";
            IEnumerable<int> x = _db.ExecuteQuery<int>(query);
            if (!x.Any())
                throw new Exception(String.Format($"{query} not found"));
        }

        string ValidateFileName(string text)
        {
            string fileName = Path.GetFileName(text);
            fileName = DataUtils.VerifyUniqueUrlFileName(Action.AttachmentPath, fileName);
            return RemoveSpecialCharacters(fileName);
        }

        static string RemoveSpecialCharacters(string text)
        {
            return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
        }

        static string VerifyUniqueUrlFileName(string directory, string fileName)
        {
            string path = Path.Combine(directory, fileName);
            string result = fileName;
            int i = 0;
            while (File.Exists(path))
            {
                i++;
                if (i > 20) break;
                string name = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);
                result = name + "_" + i.ToString() + ext;
                path = Path.Combine(directory, result);
            }

            return result;
        }

        //public ActionAttachmentModel(string fileName, string description)
        //{
        //    _proxy = new AttachmentProxy()
        //    {
        //        //AttachmentID =
        //        OrganizationID = Action.Ticket.User.Organization.OrganizationID,
        //        FileName = fileName,
        //        //FileType =
        //        //FileSize =
        //        //Path =
        //        Description = description,
        //        DateCreated = DateTime.UtcNow,
        //        //DateModified =
        //        CreatorID = Action.Ticket.User.UserID,
        //        //ModifierID =
        //        RefType = ReferenceType.Actions,
        //        RefID = Action.ActionID,
        //        //CreatorName = Action.Ticket.User.CreatorName,
        //        //SentToJira =
        //        //ProductFamilyID =
        //        //ProductFamily =
        //        //SentToTFS =
        //        //SentToSnow =
        //        //FilePathID =
        //    };
        //}

    }
}

