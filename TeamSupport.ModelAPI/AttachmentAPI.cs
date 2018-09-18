﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TeamSupport.Data;
using TeamSupport.IDTree;
using System.IO;
using TeamSupport.DataAPI;
using System.Diagnostics;
using System.Data.Linq;

namespace TeamSupport.ModelAPI
{
    public class AttachmentAPI
    {
        /// <summary> Create attachment files </summary>
        public static List<AttachmentProxy> CreateAttachments(HttpContext context, out string _ratingImage)
        {
            List<AttachmentProxy> result = new List<AttachmentProxy>();
            GetPathMap(context, out PathMap pathMap, out int refID, out _ratingImage);
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    // valid ID to add attachment
                    IAttachmentParent model = ClassFactory(connection, pathMap._refType, refID);
                    HttpFileCollection files = context.Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (files[i].ContentLength <= 0)
                            continue;

                        AttachmentFile attachmentFile = new AttachmentFile(model, files[i]);    // create the file
                        AttachmentProxy proxy = AttachmentProxy.ClassFactory(pathMap._refType);  // construct the proxy
                        proxy.RefID = refID;
                        InitializeProxy(context, connection, attachmentFile, proxy);
                        DataAPI.Data_API.Create(model.AsIDNode, proxy);  // save proxy to DB
                        result.Add(proxy);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO - tell user we failed 
                DataAPI.Data_API.LogMessage(ActionLogType.Insert, ReferenceType.None, 0, "choke", ex);
            }
            return result;
        }

        public static TProxy Read<TProxy>(IDNode node) where TProxy : class
        {
            TProxy tProxy = default(TProxy);
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    switch (typeof(TProxy).Name) // alphabetized list
                    {
                        case "AttachmentProxy": // read all attachment types
                            {
                                ActionAttachmentModel attachment = (ActionAttachmentModel)node;
                                Table<AttachmentProxy> table = attachment.Connection._db.GetTable<AttachmentProxy>();
                                tProxy = table.Where(a => a.AttachmentID == attachment.ActionAttachmentID).First() as TProxy;
                            }
                            break;
                        case "AttachmentProxy[]": // action attachments
                            {
                                ActionModel action = (ActionModel)node;
                                Table<AttachmentProxy> table = node.Connection._db.GetTable<AttachmentProxy>();
                                tProxy = table.Where(a => a.RefID == action.ActionID && a.RefType == AttachmentProxy.References.Actions).ToArray() as TProxy;
                            }
                            break;
                        default:
                            if (Debugger.IsAttached) Debugger.Break();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                int logid = Data_API.LogException(new Proxy.AuthenticationModel(), ex, "Attachment Read Exception:" + ex.Source);
            }
            return tProxy;
        }


        static IAttachmentParent ClassFactory(ConnectionContext connection, AttachmentProxy.References refType, int refID)
        {
            switch (refType)
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

        public static void DeleteActionAttachment(int attachmentID)
        {
            try
            {
                using (ConnectionContext connection = new ConnectionContext())
                {
                    // user have permission to modify this action?
                    ActionAttachmentModel attachment = new ActionAttachmentModel(connection, attachmentID);
                    if (!attachment.Action.CanEdit())
                        return;

                    AttachmentProxy proxy = Data_API.Read<AttachmentProxy>(attachment);
                    AttachmentFile file = new AttachmentFile(attachment, proxy);
                    Data_API.Delete(attachment); // remove from database
                    file.Delete();  // delete file
                }
            }
            catch (Exception ex)
            {
                Data_API.LogMessage(ActionLogType.Delete, ReferenceType.Attachments, attachmentID, "Unable to delete attachment", ex);
            }
        }


        #region PathMap
        public struct PathMap
        {
            public string _path;
            public AttachmentProxy.References _refType;
            public PathMap(string path, AttachmentProxy.References refType)
            {
                _path = path;
                _refType = refType;
            }
        }
        static readonly PathMap[] _pathMap;

        /// <summary> From AttachmentPath </summary>
        static AttachmentAPI()
        {
            _pathMap = new PathMap[]{
                    new PathMap("Actions", AttachmentProxy.References.Actions),
                    new PathMap("AgentRating", AttachmentProxy.References.None),
                    new PathMap("AssetAttachments", AttachmentProxy.References.Assets),
                    new PathMap("Images\\Chat", AttachmentProxy.References.None),
                    new PathMap("ChatAttachments", AttachmentProxy.References.ChatAttachments),
                    new PathMap("Styles\\Chat", AttachmentProxy.References.None),
                    new PathMap("CustomerActivityAttachments", AttachmentProxy.References.CompanyActivity),
                    new PathMap("ContactActivityAttachments", AttachmentProxy.References.ContactActivity),
                    new PathMap("Images\\Avatars\\Contacts", AttachmentProxy.References.None),
                    new PathMap("Images\\HubLogo", AttachmentProxy.References.CustomerHubLogo),
                    new PathMap("Images", AttachmentProxy.References.None),
                    new PathMap("Imports", AttachmentProxy.References.Imports),
                    new PathMap("Imports\\Logs", AttachmentProxy.References.None),
                    new PathMap("Organizations", AttachmentProxy.References.Organizations),
                    new PathMap("OrganizationAttachments", AttachmentProxy.References.Organizations),
                    new PathMap("Images\\CompanyLogo", AttachmentProxy.References.None),
                    new PathMap("Products", AttachmentProxy.References.ProductVersions),
                    new PathMap("Images\\Avatars", AttachmentProxy.References.None),
                    new PathMap("ScheduledReports", AttachmentProxy.References.None),
                    new PathMap("ScheduledReports\\Logs", AttachmentProxy.References.None),
                    new PathMap("Styles", AttachmentProxy.References.None),
                    new PathMap("Tasks", AttachmentProxy.References.Tasks),
                    new PathMap("Images\\Temp", AttachmentProxy.References.None),
                    new PathMap("Images\\TicketTypes", AttachmentProxy.References.None),
                    new PathMap("UserAttachments", AttachmentProxy.References.Users),
                    new PathMap("WaterCooler", AttachmentProxy.References.WaterCooler),
            };
        }

        /// <summary> ...\Upload\Actions\55582993 </summary>
        static void GetPathMap(HttpContext context, out PathMap pathMap, out int id, out string ratingImage)
        {
            List<string> segments = new List<string>();
            foreach (string segment in context.Request.Url.Segments)
                segments.Add(segment.ToLower().Trim().Replace("/", ""));

            // id
            if (!int.TryParse(segments[segments.Count - 1], out id))
                throw new Exception($"Bad attachment id {segments[segments.Count - 1]}");
            segments.RemoveAt(segments.Count - 1);

            // _ratingImage
            if (segments[segments.Count - 1] == "ratingpositive" || segments[segments.Count - 1] == "ratingneutral" || segments[segments.Count - 1] == "ratingnegative")
            {
                ratingImage = segments[segments.Count - 1];
                segments.RemoveAt(segments.Count - 1);
            }
            else
                ratingImage = "";

            // pathMap
            string path = null;
            bool pathStart = false;
            foreach (string segment in segments)
            {
                switch (segment)
                {
                    case "upload":
                    case "chatupload":
                        pathStart = true;
                        break;
                    default:
                        if(!pathStart)
                            break;
                        if (path == null)
                            path = segment;
                        else
                            path += '\\' + segment;
                        break;
                }
            }
            pathMap = _pathMap.Where(p => path == p._path.ToLower()).First(); // throw if not found
        }

        private static void InitializeProxy(HttpContext context, ConnectionContext connection, AttachmentFile attachmentFile, AttachmentProxy proxy)
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
            proxy.DateCreated = proxy.DateModified = DateTime.UtcNow;
        }

        #endregion

    }
}
