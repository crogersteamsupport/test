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
        List<string> segments = GetUrlSegments(context);
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

        AttachmentPath.Folder folder = GetFolder(context, segments.ToArray());
        if (folder == AttachmentPath.Folder.None) throw new Exception("Invalid path.");
        SaveFiles(context, folder);
      }
      catch (Exception ex)
      {
        context.Response.Write(ex.Message);
      }
      
      context.Response.End();
    }

    private List<string> GetUrlSegments(HttpContext context)
    {
        List<string> segments = new List<string>();
        bool flag = false;
        for (int i = 0; i < context.Request.Url.Segments.Length; i++)
        {
          string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
          if (flag) segments.Add(s);
          if (s == "upload") flag = true;
        }
       return segments;
    }


    private void SaveFiles(HttpContext context, AttachmentPath.Folder folder)
    {
      ReferenceType refType = AttachmentPath.GetFolderReferenceType(folder);
      if (refType == ReferenceType.None)
      {
        SaveFilesOld(context, folder);
        return;
      }

      List<UploadResult> result = new List<UploadResult>();

      string path = AttachmentPath.GetPath(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, folder);
      if (_id != null) path = Path.Combine(path, _id.ToString());
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      HttpFileCollection files = context.Request.Files;
      
      for (int i = 0; i < files.Count; i++)
      {
        if (files[i].ContentLength > 0)
        {
          string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));

          if (_ratingImage != "")
          {

          }

          files[i].SaveAs(Path.Combine(path, fileName));
          if (refType != ReferenceType.None && _id != null)
          {
            Attachment attachment = (new Attachments(TSAuthentication.GetLoginUser())).AddNewAttachment();
            attachment.RefType = refType;
            attachment.RefID = (int)_id;
            attachment.OrganizationID = TSAuthentication.OrganizationID;
            attachment.FileName = fileName;
            attachment.Path = Path.Combine(path, fileName);
            attachment.FileType = files[i].ContentType;
            attachment.FileSize = files[i].ContentLength;
            if (context.Request.Form["description"] != null)
                attachment.Description = context.Request.Form["description"].Replace("\n","<br />");

            result.Add(new UploadResult(fileName, attachment.FileType, attachment.FileSize));
            attachment.Collection.Save();
          }
          else
          {
            switch (refType)
            {
              case ReferenceType.Imports:
                Import import = (new Imports(TSAuthentication.GetLoginUser())).AddNewImport();
                import.RefType = (ReferenceType)Convert.ToInt32(context.Request.Form["refType"]);
                import.FileName = fileName;
                import.OrganizationID = TSAuthentication.OrganizationID;
                result.Add(new UploadResult(fileName, files[i].ContentType, files[i].ContentLength));
                import.Collection.Save();
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

    private void SaveFilesOld(HttpContext context, AttachmentPath.Folder folder)
    {
      StringBuilder builder = new StringBuilder();
      string path = AttachmentPath.GetPath(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID, folder);
      if (_id != null) path = Path.Combine(path, _id.ToString());
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      HttpFileCollection files = context.Request.Files;
      for (int i = 0; i < files.Count; i++)
			{
        if(files[i].ContentLength > 0)
        {
          string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));
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

    private string RemoveSpecialCharacters(string text)
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

    private AttachmentPath.Folder GetFolder(HttpContext context, string[] segments)
    {
      StringBuilder path = new StringBuilder();
      for (int i = 0; i < segments.Length; i++)
      {
        path.Append("\\");
        path.Append(segments[i]);
      }
      return AttachmentPath.GetFolderByName(path.ToString());
    }

    #endregion
  }

  [DataContract]
  public class UploadResult
  {
    public UploadResult(string name, string type, long size)
    {
      this.name = name;
      this.size = size;
      this.type = type;
    }

    [DataMember]
    public string name { get; set; }
    [DataMember]
    public long size { get; set; }
    [DataMember]
    public string type { get; set; }
  }
}
