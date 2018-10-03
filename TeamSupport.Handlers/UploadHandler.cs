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
    class UploadHandler : IHttpHandler, IRequiresSessionState
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        private int? _id = null;
        private string _ratingImage = "";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            try
            {
                {
                    List<AttachmentProxy> proxies = ModelAPI.AttachmentAPI.CreateAttachments(context, out _ratingImage);
                    //if (proxies != null)    // SCOT fall through if not supported by RefType infrastructure
                    {
                        context.Response.Clear();
                        context.Response.ContentType = "text/plain";
                        List<UploadResult> result = new List<UploadResult>();
                        foreach (AttachmentProxy attachment in proxies)
                            result.Add(new UploadResult(attachment.FileName, attachment.FileType, attachment.FileSize, attachment.AttachmentID));
                        context.Response.ContentType = "text/html";
                        context.Response.Write(DataUtils.ObjectToJson(result.ToArray()));
                        return;
                    }
                }

                if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                List<string> segments = UploadUtils.GetUrlSegments(context);
                TeamSupport.Data.Quarantine.UploadHandlerQ.ProcessRequest(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, context, _id, _ratingImage, segments);



            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }

            context.Response.End();
        }


        #endregion
    }

}
