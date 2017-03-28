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
                List<string> segments = UploadUtils.GetUrlSegments(context);
                int id;
                if (int.TryParse(segments[segments.Count - 1], out id))
                {
                    _id = id;
                    segments.RemoveAt(segments.Count - 1);
                }

                if (segments[segments.Count - 1] == "ratingpositive" || segments[segments.Count - 1] == "ratingneutral" || segments[segments.Count - 1] == "ratingnegative")
                {
                    _ratingImage = segments[segments.Count - 1];
                    segments.RemoveAt(segments.Count - 1);
                }

                AttachmentPath.Folder folder = UploadUtils.GetFolder(context, segments.ToArray());
                if (folder == AttachmentPath.Folder.None) throw new Exception("Invalid path.");

                ReferenceType refType = AttachmentPath.GetFolderReferenceType(folder);
                if (refType == ReferenceType.None)
                {
                    SaveFilesOld(context, folder);
                }
                else
                {
                    UploadUtils.SaveFiles(context, folder, TSAuthentication.OrganizationID, _id);
                }

            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }

            context.Response.End();
        }



        private void SaveFilesOld(HttpContext context, AttachmentPath.Folder folder)
        {
            StringBuilder builder = new StringBuilder();
            string path = AttachmentPath.GetPath(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, folder);
            if (_id != null) path = Path.Combine(path, _id.ToString());
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            HttpFileCollection files = context.Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].ContentLength > 0)
                {
                    string fileName =  UploadUtils.RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));
                    if (builder.Length > 0) builder.Append(",");
                    builder.Append(fileName);

                    if (_ratingImage != "")
                    {
                        fileName = _ratingImage + ".png";
                        AgentRatingsOptions ratingoptions = new AgentRatingsOptions(TSAuthentication.GetLoginUser());
                        ratingoptions.LoadByOrganizationID(TSAuthentication.OrganizationID);

                        if (ratingoptions.IsEmpty)
                        {
                            AgentRatingsOption opt = (new AgentRatingsOptions(TSAuthentication.GetLoginUser())).AddNewAgentRatingsOption();
                            opt.OrganizationID = TSAuthentication.OrganizationID;
                            switch (_ratingImage)
                            {
                                case "ratingpositive":
                                    opt.PositiveImage = "/dc/" + TSAuthentication.OrganizationID + "/agentrating/ratingpositive";
                                    break;
                                case "ratingneutral":
                                    opt.NeutralImage = "/dc/" + TSAuthentication.OrganizationID + "/agentrating/ratingneutral";
                                    break;
                                case "ratingnegative":
                                    opt.NegativeImage = "/dc/" + TSAuthentication.OrganizationID + "/agentrating/ratingnegative";
                                    break;
                            }
                            opt.Collection.Save();
                        }
                        else
                        {
                            switch (_ratingImage)
                            {
                                case "ratingpositive":
                                    ratingoptions[0].PositiveImage = "/dc/" + TSAuthentication.OrganizationID + "/agentrating/ratingpositive";
                                    break;
                                case "ratingneutral":
                                    ratingoptions[0].NeutralImage = "/dc/" + TSAuthentication.OrganizationID + "/agentrating/ratingneutral";
                                    break;
                                case "ratingnegative":
                                    ratingoptions[0].NegativeImage = "/dc/" + TSAuthentication.OrganizationID + "/agentrating/ratingnegative";
                                    break;
                            }
                            ratingoptions[0].Collection.Save();
                        }



                    }

                    fileName = Path.Combine(path, fileName);
                    files[i].SaveAs(fileName);
                }
            }
            context.Response.Write(builder.ToString());
        }

        #endregion
    }

}
