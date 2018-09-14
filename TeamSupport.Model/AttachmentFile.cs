﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace TeamSupport.Model
{
    /// <summary>
    /// The attachment file is stored at AttachmentPath
    /// </summary>
    public class AttachmentFile
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public long ContentLength { get; private set; }
        public string ContentType { get; private set; }

        /// <summary> New file </summary>
        public AttachmentFile(ActionModel actionModel, HttpPostedFile postedFile)
        {
            FileName = VerifyFileName(actionModel.AttachmentPath, postedFile.FileName);
            FilePath = Path.Combine(actionModel.AttachmentPath, FileName);
            ContentType = postedFile.ContentType;
            ContentLength = postedFile.ContentLength;
            postedFile.SaveAs(FilePath);    // write file to disk
        }

        /// <summary> Existing file </summary>
        public AttachmentFile(ActionAttachment attachment, Data.AttachmentProxy proxy)
        {
            FileInfo file = new FileInfo(proxy.Path);
            DirectoryInfo dir = file.Directory;
            string dirName = file.Directory.ToString();
            if (!dirName.Equals(attachment.Action.AttachmentPath))
                throw new Exception($"File path {file.Directory.Name} != {attachment.Action.AttachmentPath}");

            FileName = proxy.FileName;
            FilePath = proxy.Path;
            ContentLength = proxy.FileSize;
            ContentType = proxy.FileType;
        }

        /// <summary> Delete attachment file </summary>
        public void Delete()
        {
            string filePath = FilePath;
            FileName = null;
            FilePath = null;
            ContentLength = 0;
            ContentType = null;
            File.Delete(filePath);
        }

        public Data.AttachmentProxy AsAttachmentProxy(HttpRequest request, ActionModel actionModel)
        {
            string description = request.Form["description"];
            if (description != null)
                description = description.Replace("\n", "<br />");

            int? productFamilyID = null;
            string tmp = request.Form["productFamilyID"];
            if ((tmp != null) && !tmp.Equals("-1"))
                productFamilyID = Int32.Parse(tmp);

            DateTime now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            UserSession user = actionModel.Ticket.User;
            Data.AttachmentProxy proxy = new Data.AttachmentProxy()
            {
                FilePathID = ActionModel.ActionPathIndex,
                //SentToSnow = ,
                //SentToTFS = ,
                ProductFamilyID = productFamilyID,
                //SentToJira = ,
                RefID = actionModel.ActionID,
                RefType = Data.ReferenceType.Actions,
                ModifierID = user.UserID,
                CreatorID = user.UserID,
                Description = description,
                Path = FilePath,
                FileSize = ContentLength,
                FileType = ContentType,
                FileName = FileName,
                OrganizationID = user.Organization.OrganizationID,
                //AttachmentID = this.AttachmentID,
                //CreatorName = Action.Ticket.User.CreatorName(),
                DateCreated = now,
                DateModified = now
            };
            return proxy;
        }

        static string VerifyFileName(string directory, string text)
        {
            string fileName = Path.GetFileName(text);
            fileName = VerifyFileName_Unique(directory, fileName);
            return VerifyFileName_SpecialCharacters(fileName);
        }

        static string VerifyFileName_Unique(string directory, string fileName)
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

        static string VerifyFileName_SpecialCharacters(string text)
        {
            return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
        }
    }
}
