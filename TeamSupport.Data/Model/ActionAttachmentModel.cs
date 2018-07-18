using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.IO;
using System.Web;

namespace TeamSupport.Data.Model
{
    public class UploadResult
    {
        public string name;
        public long size;
        public string type;
        public int id;

        public UploadResult(string name, string type, long size, int id = 0)
        {
            this.name = name;
            this.size = size;
            this.type = type;
            this.id = id;
        }
    }

    /// <summary>
    /// see 
    /// </summary>
    public class ActionAttachmentModel : AttachmentProxy
    {
        public ActionModel Action { get; private set; }
        public DataContext _db { get; private set; }
        public AttachmentPath.Folder Folder { get; private set; }
        string _filePath;

        public ActionAttachmentModel(ActionModel action)
        {
            Action = action;
        }

        //GetAttachmentPath(int filePathID)
        //{
        //    string root = _db.ExecuteQuery<string>($"SELECT Value FROM FilePaths WHERE ID={filePathID}").FirstOrDefault();
        //    return System.IO.Path.Combine(System.IO.Path.Combine(root, "Actions"), Action.ActionID.ToString()) + "\\";
        //}

        public void CreateAttachment(string fileName, string description)
        {
            //string directory = TSUtils.GetAttachmentPath(folderName, _refID, 3);
            //string fileName = file.GetName();
            //fileName = Path.GetFileName(fileName);
            //fileName = DataUtils.VerifyUniqueFileName(directory, fileName);
            //Directory.CreateDirectory(directory);
            //file.SaveAs(attachment.Path, true);

            AttachmentProxy attachment = new AttachmentProxy()
            {
                //AttachmentID =
                OrganizationID = Action.Ticket.User.Organization.OrganizationID,
                FileName = fileName,
                //FileType =
                //FileSize =
                //Path =
                Description = description,
                DateCreated = DateTime.UtcNow,
                //DateModified =
                CreatorID = Action.Ticket.User.UserID,
                //ModifierID =
                RefType = ReferenceType.Actions,
                RefID = Action.ActionID,
                CreatorName = Action.Ticket.User.CreatorName,
                //SentToJira =
                //ProductFamilyID =
                //ProductFamily =
                //SentToTFS =
                //SentToSnow =
                //FilePathID =
            };
        }

        //public static void SaveFiles(HttpContext context, AttachmentPath.Folder folder, int organizationID, int? itemID)
        //{
        //    ReferenceType refType = AttachmentPath.GetFolderReferenceType(folder);
        //    List<UploadResult> result = new List<UploadResult>();

        //    string path = AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, folder, 3);
        //    if (itemID != null) path = Path.Combine(path, itemID.ToString());
        //    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        //    HttpFileCollection files = context.Request.Files;

        //    for (int i = 0; i < files.Count; i++)
        //    {
        //        if (files[i].ContentLength > 0)
        //        {
        //            string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));

        //            files[i].SaveAs(Path.Combine(path, fileName));
        //            if (refType != ReferenceType.None && itemID != null)
        //            {
        //                Attachment attachment = (new Attachments(TSAuthentication.GetLoginUser())).AddNewAttachment();
        //                attachment.RefType = refType;
        //                attachment.RefID = (int)itemID;
        //                attachment.OrganizationID = organizationID;
        //                attachment.FileName = fileName;
        //                attachment.Path = Path.Combine(path, fileName);
        //                attachment.FileType = files[i].ContentType;
        //                attachment.FileSize = files[i].ContentLength;
        //                attachment.FilePathID = 3;
        //                if (context.Request.Form["description"] != null)
        //                    attachment.Description = context.Request.Form["description"].Replace("\n", "<br />");
        //                if (context.Request.Form["productFamilyID"] != null && context.Request.Form["productFamilyID"] != "-1")
        //                    attachment.ProductFamilyID = Int32.Parse(context.Request.Form["productFamilyID"]);

        //                attachment.Collection.Save();
        //                result.Add(new UploadResult(fileName, attachment.FileType, attachment.FileSize, attachment.AttachmentID));
        //            }
        //            else
        //            {
        //                switch (refType)
        //                {
        //                    case ReferenceType.Imports:
        //                        //Not saving import till user click on import button saving both imports and mappings
        //                        //Import import = (new Imports(TSAuthentication.GetLoginUser())).AddNewImport();
        //                        //import.RefType = (ReferenceType)Convert.ToInt32(context.Request.Form["refType"]);
        //                        //import.FileName = fileName;
        //                        //import.OrganizationID = TSAuthentication.OrganizationID;
        //                        //import.Collection.Save();
        //                        result.Add(new UploadResult(fileName, files[i].ContentType, files[i].ContentLength));
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    context.Response.Clear();
        //    context.Response.ContentType = "text/plain";
        //    context.Response.Write(DataUtils.ObjectToJson(result.ToArray()));
        //}


    }
}
