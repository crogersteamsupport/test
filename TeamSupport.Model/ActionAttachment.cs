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
            ActionAttachmentID = InsertActionAttachment(user, postedFile, request, File.FileName, File.FilePath); // add ActionAttachment record
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
        private int InsertActionAttachment(Data.LoginUser user, HttpPostedFile postedFile, HttpRequest request, string fileName, string filePath)
        {
            string description = request.Form["description"];
            if(description != null)
                description = description.Replace("\n", "<br />");

            int? productFamilyID = null;
            string tmp = request.Form["productFamilyID"];
            if ((tmp != null) && !tmp.Equals("-1"))
                productFamilyID = Int32.Parse(tmp);

            DateTime now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            Data.AttachmentProxy proxy = new Data.AttachmentProxy()
            {
                FilePathID = 3,
                //SentToSnow = ,
                //SentToTFS = ,
                ProductFamilyID = productFamilyID,
                //SentToJira = ,
                RefID = Action.ActionID,
                RefType = Data.ReferenceType.Actions,
                ModifierID = user.UserID,
                CreatorID = user.UserID,
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

            // hard code all the numbers, parameterize all the strings so they are SQL-Injection checked
            string query = "INSERT INTO ActionAttachments(OrganizationID, FileName, FileType, FileSize, Path, DateCreated, DateModified, CreatorID, ModifierID, ActionID, SentToJira, SentToTFS, SentToSnow, FilePathID) " +
                $"VALUES({proxy.OrganizationID}, {{0}}, {{1}}, {proxy.FileSize}, {{2}}, '{proxy.DateCreated}', '{proxy.DateModified}', {proxy.CreatorID}, {proxy.ModifierID}, {proxy.RefID}, {(proxy.SentToJira?1:0)}, {(proxy.SentToTFS?1:0)}, {(proxy.SentToSnow?1:0)}, {proxy.FilePathID})" +
                "SELECT SCOPE_IDENTITY()";
            decimal value = _db.ExecuteQuery<decimal>(query, proxy.FileName, proxy.FileType, proxy.Path).Min();
            return Decimal.ToInt32(value);
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

