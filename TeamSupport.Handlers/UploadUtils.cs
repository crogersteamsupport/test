using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;
using System.Drawing;
using TeamSupport.Data;
using System.IO;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;

namespace TeamSupport.Handlers
{
    public class UploadUtils
    {
        public static List<string> GetUrlSegments(HttpContext context, string uploadFlag = "upload")
        {
            List<string> segments = new List<string>();
            bool flag = false;
            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
                string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
                if (flag) segments.Add(s);
                if (s == uploadFlag) flag = true;
            }
            return segments;
        }

        public static void SaveFiles(HttpContext context, AttachmentPath.Folder folder, int organizationID, int? itemID)
        {
            ReferenceType refType = AttachmentPath.GetFolderReferenceType(folder);
            List<UploadResult> result = new List<UploadResult>();

            string path = AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, folder);
            if (itemID != null) path = Path.Combine(path, itemID.ToString());
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            HttpFileCollection files = context.Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].ContentLength > 0)
                {
                    string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));

                    files[i].SaveAs(Path.Combine(path, fileName));
                    if (refType != ReferenceType.None && itemID != null)
                    {
                        Attachment attachment = (new Attachments(TSAuthentication.GetLoginUser())).AddNewAttachment();
                        attachment.RefType = refType;
                        attachment.RefID = (int)itemID;
                        attachment.OrganizationID = organizationID;
                        attachment.FileName = fileName;
                        attachment.Path = Path.Combine(path, fileName);
                        attachment.FileType = files[i].ContentType;
                        attachment.FileSize = files[i].ContentLength;
                        if (context.Request.Form["description"] != null)
                            attachment.Description = context.Request.Form["description"].Replace("\n", "<br />");
                        if (context.Request.Form["productFamilyID"] != null && context.Request.Form["productFamilyID"] != "-1")
                            attachment.ProductFamilyID = Int32.Parse(context.Request.Form["productFamilyID"]);

                        attachment.Collection.Save();
                        result.Add(new UploadResult(fileName, attachment.FileType, attachment.FileSize, attachment.AttachmentID));
                    }
                    else
                    {
                        switch (refType)
                        {
                            case ReferenceType.Imports:
                                //Not saving import till user click on import button saving both imports and mappings
                                //Import import = (new Imports(TSAuthentication.GetLoginUser())).AddNewImport();
                                //import.RefType = (ReferenceType)Convert.ToInt32(context.Request.Form["refType"]);
                                //import.FileName = fileName;
                                //import.OrganizationID = TSAuthentication.OrganizationID;
                                //import.Collection.Save();
                                result.Add(new UploadResult(fileName, files[i].ContentType, files[i].ContentLength));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            context.Response.Clear();
            context.Response.ContentType = "text/plain";
            context.Response.Write(DataUtils.ObjectToJson(result.ToArray()));
        }

        public static string RemoveSpecialCharacters(string text)
        {
            return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
            //StringBuilder builder = new StringBuilder();
            //foreach (char c in text)
            //  {
            //  if (!char.IsLetterOrDigit(c) && c != '.' && c != '@') builder.Append("_");
            //  else builder.Append(c);
            //  }
            //return builder.ToString();

        }

        public static AttachmentPath.Folder GetFolder(HttpContext context, string[] segments)
        {
            StringBuilder path = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                path.Append("\\");
                path.Append(segments[i]);
            }
            return AttachmentPath.GetFolderByName(path.ToString());
        }

    }

    [DataContract]
    public class UploadResult
    {
        public UploadResult(string name, string type, long size)
        {
            this.name = name;
            this.size = size;
            this.type = type;
            this.id = 0;
        }

        public UploadResult(string name, string type, long size, int id)
        {
            this.name = name;
            this.size = size;
            this.type = type;
            this.id = id;
        }

        [DataMember]
        public string name { get; set; }
        [DataMember]
        public long size { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int id { get; set; }
    }
}
