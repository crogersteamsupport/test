using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace TeamSupport.Data.Quarantine
{
    public static class WebAppQ
    {
        public static string GetAttachmentPath1(LoginUser loginUser, Imports import)
        {
            return AttachmentPath.GetPath(loginUser, loginUser.OrganizationID, AttachmentPath.Folder.ImportLogs, import[0].FilePathID);
        }

        public static AttachmentProxy[] GetAttachmentProxies(int refID, ReferenceType refType, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReference(refType, refID);

            return attachments.GetAttachmentProxies();
        }

        public static AttachmentProxy[] GetAttachmentProxies1(int refID, ReferenceType refType, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReference(refType, refID, "DateCreated desc");

            return attachments.GetAttachmentProxies();
        }

        public static AttachmentProxy[] GetAttachmentProxies2(int refID, ReferenceType refType, bool includeChildren, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReference(refType, refID, "DateCreated desc", includeChildren);

            return attachments.GetAttachmentProxies();
        }

        public static AttachmentProxy[] GetAttachmentProxies3(int refID, ReferenceType refType, bool includeChildren, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReferenceAndUserRights(refType, refID, loginUser.UserID, "DateCreated desc", includeChildren);

            return attachments.GetAttachmentProxies();
        }

        public static string GetCsvFile(string uploadedFileName, LoginUser loginUser)
        {
            return Path.Combine(AttachmentPath.GetPath(loginUser, loginUser.OrganizationID, AttachmentPath.Folder.Imports, 3), uploadedFileName);
        }

        public static AttachmentProxy[] GetAttachmentProxies4(int versionID, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReference(ReferenceType.ProductVersions, versionID);
            return attachments.GetAttachmentProxies();
        }

        public static AttachmentProxy[] GetAttachmentProxies5(int reminderID, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByReference(ReferenceType.Tasks, reminderID);
            return attachments.GetAttachmentProxies();
        }

        public static AttachmentProxy[] GetAttachmentProxies6(int refID, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByActionID(refID);
            return attachments.GetAttachmentProxies();
        }

        public static AttachmentProxy[] GetAttachmentProxies7(int ticketID, LoginUser loginUser)
        {
            Attachments attach = new Attachments(loginUser);
            attach.LoadByTicketId(ticketID);
            AttachmentProxy[] proxies = attach.GetAttachmentProxies();
            return proxies;
        }

        public static AttachmentProxy[] GetAttachmentProxies8(int actionID, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByActionID(actionID);
            return attachments.GetAttachmentProxies();
        }

        public static void CopyInsertedKBAttachments1(int actionID, int insertedKBTicketID, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadKBByTicketID(insertedKBTicketID);
            if (attachments.Count > 0)
            {
                Attachments clonedAttachments = new Attachments(loginUser);
                foreach (Attachment attachment in attachments)
                {
                    Attachment clonedAttachment = clonedAttachments.AddNewAttachment();
                    clonedAttachment.OrganizationID = attachment.OrganizationID;
                    clonedAttachment.FileType = attachment.FileType;
                    clonedAttachment.FileSize = attachment.FileSize;
                    clonedAttachment.Description = attachment.Description;
                    clonedAttachment.DateCreated = attachment.DateCreatedUtc;
                    clonedAttachment.DateModified = attachment.DateModifiedUtc;
                    clonedAttachment.CreatorID = attachment.CreatorID;
                    clonedAttachment.ModifierID = attachment.ModifierID;
                    clonedAttachment.RefType = attachment.RefType;
                    clonedAttachment.SentToJira = attachment.SentToJira;
                    clonedAttachment.ProductFamilyID = attachment.ProductFamilyID;
                    clonedAttachment.FileName = attachment.FileName;
                    clonedAttachment.RefID = actionID;
                    clonedAttachment.FilePathID = attachment.FilePathID;

                    string originalAttachmentRefID = attachment.RefID.ToString();
                    string clonedActionAttachmentPath = attachment.Path.Substring(0, attachment.Path.IndexOf(@"\Actions\") + @"\Actions\".Length)
                                                        + actionID.ToString()
                                                        + attachment.Path.Substring(attachment.Path.IndexOf(originalAttachmentRefID) + originalAttachmentRefID.Length);

                    if (!Directory.Exists(Path.GetDirectoryName(clonedActionAttachmentPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(clonedActionAttachmentPath));
                    }

                    clonedAttachment.Path = clonedActionAttachmentPath;

                    File.Copy(attachment.Path, clonedAttachment.Path);

                }
                clonedAttachments.BulkSave();

            }
        }

        public static string GetAttachmentPath2(int organizationID, LoginUser loginUser)
        {
            return AttachmentPath.GetPath(loginUser, organizationID, AttachmentPath.Folder.Images);
        }

        public static string GetAttachmentPath4(int organizationID, LoginUser loginUser)
        {
            return AttachmentPath.GetPath(loginUser, organizationID, AttachmentPath.Folder.Images);
        }

        public static AttachmentProxy[] GetAttachmentProxies9(int msgid, LoginUser loginUser)
        {
            Attachments attachments = new Attachments(loginUser);
            attachments.LoadByWatercoolerID(msgid);
            return attachments.GetAttachmentProxies();
        }

        /*
 TeamSupport.Data.Quarantine.WebAppQ.
 */

    }
}
