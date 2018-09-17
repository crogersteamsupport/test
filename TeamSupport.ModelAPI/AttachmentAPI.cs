using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TeamSupport.Data;
using TeamSupport.IDTree;
using System.IO;

namespace TeamSupport.ModelAPI
{
    public class AttachmentAPI
    {
        public struct PathMap
        {
            public Folder _folder;
            public string _path;
            public AttachmentProxy.References _refType;
            public PathMap(Folder folder, string path, AttachmentProxy.References refType)
            {
                _folder = folder;
                _path = path;
                _refType = refType;
            }
        }
        static readonly PathMap[] _stuff;

        /// <summary> From AttachmentPath </summary>
        static AttachmentAPI()
        {
            _stuff = new PathMap[]{
                    new PathMap(Folder.Actions,"Actions", AttachmentProxy.References.Actions),
                    new PathMap(Folder.AgentRating,"AgentRating", AttachmentProxy.References.None),
                    new PathMap(Folder.AssetAttachments,"AssetAttachments", AttachmentProxy.References.Assets),
                    new PathMap(Folder.ChatImages,"Images\\Chat", AttachmentProxy.References.None),
                    new PathMap(Folder.ChatUploads,"ChatAttachments", AttachmentProxy.References.ChatAttachments),
                    new PathMap(Folder.ChatStyles,"Styles\\Chat", AttachmentProxy.References.None),
                    new PathMap(Folder.CompanyActivityAttachments,"CustomerActivityAttachments", AttachmentProxy.References.CompanyActivity),
                    new PathMap(Folder.ContactActivityAttachments,"ContactActivityAttachments", AttachmentProxy.References.ContactActivity),
                    new PathMap(Folder.ContactImages,"Images\\Avatars\\Contacts", AttachmentProxy.References.None),
                    new PathMap(Folder.CustomerHubLogo,"Images\\HubLogo", AttachmentProxy.References.CustomerHubLogo),
                    new PathMap(Folder.Images,"Images", AttachmentProxy.References.None),
                    new PathMap(Folder.Imports,"Imports", AttachmentProxy.References.Imports),
                    new PathMap(Folder.ImportLogs,"Imports\\Logs", AttachmentProxy.References.None),
                    new PathMap(Folder.Organizations,"Organizations", AttachmentProxy.References.Organizations),
                    new PathMap(Folder.OrganizationAttachments,"OrganizationAttachments", AttachmentProxy.References.Organizations),
                    new PathMap(Folder.OrganizationsLogo,"Images\\CompanyLogo", AttachmentProxy.References.None),
                    new PathMap(Folder.Products,"Products", AttachmentProxy.References.ProductVersions),
                    new PathMap(Folder.ProfileImages,"Images\\Avatars", AttachmentProxy.References.None),
                    new PathMap(Folder.ScheduledReports,"ScheduledReports", AttachmentProxy.References.None),
                    new PathMap(Folder.ScheduledReportsLogs,"ScheduledReports\\Logs", AttachmentProxy.References.None),
                    new PathMap(Folder.Styles,"Styles", AttachmentProxy.References.None),
                    new PathMap(Folder.Tasks,"Tasks", AttachmentProxy.References.Tasks),
                    new PathMap(Folder.TempImages,"Images\\Temp", AttachmentProxy.References.None),
                    new PathMap(Folder.TicketTypeImages,"Images\\TicketTypes", AttachmentProxy.References.None),
                    new PathMap(Folder.UserAttachments,"UserAttachments", AttachmentProxy.References.Users),
                    new PathMap(Folder.WaterCooler,"WaterCooler", AttachmentProxy.References.WaterCooler),
            };
        }

        public static void CreateAttachments(HttpContext context, List<string> segments, int refID, string _ratingImage = "")
        {
            string path = null;
            foreach (string segment in segments)
            {
                if (path == null)
                    path = segment;
                else
                    path += '\\' + segment.Trim().ToLower();
            }
            PathMap pathMap = _stuff.Where(p => path == p._path.ToLower()).First(); // throw if not found

            Create(pathMap, context, refID);
        }

        static void Create(PathMap pathMap, HttpContext context, int refID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    // valid ID to add attachment
                    IDNode model = ClassFactory(connection, pathMap._refType, refID);
                    HttpFileCollection files = context.Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i].ContentLength <= 0)
                            continue;

                        AttachmentFile attachmentFile = new AttachmentFile(model, files[i]);
                        AttachmentProxy proxy = AttachmentProxy.ClassFactory(pathMap._refType, refID);
                        {
                            // context
                            string tmp = context.Request.Form["description"];
                            if (tmp != null)
                                proxy.Description = tmp.Replace("\n", "<br />");

                            tmp = context.Request.Form["productFamilyID"];
                            if ((tmp != null) && !tmp.Equals("-1"))
                                proxy.ProductFamilyID = Int32.Parse(tmp);
                            proxy.FilePathID = AttachmentProxy.AttachmentPathIndex;

                            // file
                            proxy.Path = attachmentFile.FilePath;
                            proxy.FileSize = attachmentFile.ContentLength;
                            proxy.FileType = attachmentFile.ContentType;
                            proxy.FileName = attachmentFile.FileName;

                            // authentication
                            proxy.OrganizationID = connection.OrganizationID;
                            proxy.ModifierID = connection.UserID;
                            proxy.CreatorID = connection.UserID;
                            DateTime now = DateTime.UtcNow;
                            proxy.DateCreated = now;
                            proxy.DateModified = now;
                        }

                        DataAPI.Data_API.Create(model, proxy);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed to read
                //Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
        }

        static IDNode ClassFactory(ConnectionContext connection, AttachmentProxy.References refType, int refID)
        {
            switch(refType)
            {
                case AttachmentProxy.References.Actions: return new ActionModel(connection, refID);
                //case AttachmentProxy.References.Assets: return new AssetModel(connection, refID);
                //case AttachmentProxy.References.ChatAttachments: return new ChatModel(connection, refID);
                //case AttachmentProxy.References.CompanyActivity: return new CompanyActivityModel(connection, refID);
                //case AttachmentProxy.References.ContactActivity: return new ContactActivityModel(connection, refID);
                //case AttachmentProxy.References.Contacts: return new ContactModel(connection, refID);
                //case AttachmentProxy.References.CustomerHubLogo: return new CustomerHubLogoModel(connection, refID);
                //case AttachmentProxy.References.Organizations: return new OrganizationModel(connection, refID);
                //case AttachmentProxy.References.ProductVersions: return new ProductVersionModel(connection, refID);
                //case AttachmentProxy.References.Tasks: return new TaskModel(connection, refID);
                //case AttachmentProxy.References.UserPhoto: return new UserPhotoModel(connection, refID);
                //case AttachmentProxy.References.Users: return new UserModel(connection, refID);
                //case AttachmentProxy.References.WaterCooler: return new WaterCoolerModel(connection, refID);
                //case AttachmentProxy.References.Imports: return new ImportsModel(connection, refID);
                default:
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                    throw new Exception($"bad ReferenceType {refType}");

            }

        }

        public enum Folder
        {
            None,
            Images,
            Styles,
            ChatImages,
            ChatStyles,
            TicketTypeImages,
            Products,
            Actions,
            Organizations,
            ProfileImages,
            WaterCooler,
            OrganizationAttachments,
            UserAttachments,
            AgentRating,
            AssetAttachments,
            Imports,
            OrganizationsLogo,
            ContactImages,
            ImportLogs,
            TempImages,
            CustomerHubLogo,
            ScheduledReports,
            ScheduledReportsLogs,
            ChatUploads,
            Tasks,
            CompanyActivityAttachments,
            ContactActivityAttachments
        };

    }
}
