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
    class ChatUploadHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            try
            {
                List<string> segments = UploadUtils.GetUrlSegments(context, "chatupload");
                int chatID = int.Parse(segments[segments.Count - 1]);
                segments.RemoveAt(segments.Count - 1);

                int orgID = int.Parse(segments[segments.Count - 1]);
                segments.RemoveAt(segments.Count - 1);

                Chat chat = Chats.GetChat(LoginUser.Anonymous, chatID);
                if (chat.OrganizationID != orgID) throw new Exception("Invalid ID");

                AttachmentPath.Folder folder = UploadUtils.GetFolder(context, segments.ToArray());
                if (folder == AttachmentPath.Folder.None) throw new Exception("Invalid path.");

                ReferenceType refType = AttachmentPath.GetFolderReferenceType(folder);
                UploadUtils.SaveFiles(context, folder, orgID, chatID);

            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }

            context.Response.End();
        }
    }


}
