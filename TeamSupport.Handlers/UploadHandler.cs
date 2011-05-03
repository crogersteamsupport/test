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

namespace TeamSupport.Handlers
{
  class UploadHandler : IHttpHandler, IRequiresSessionState
  {
    #region IHttpHandler Members

    public bool IsReusable
    {
      get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
      context.Response.ContentType = "text/html";
      try
      {
        string[] segments = GetUrlSegments(context);
        AttachmentPath.Folder folder = GetFolder(context, segments);
        if (folder == AttachmentPath.Folder.None) throw new Exception("Invalid path.");
        SaveFiles(context, folder);
      }
      catch (Exception ex)
      {
        context.Response.Write(ex.Message);
      }
      
      context.Response.End();
    }

    private string[] GetUrlSegments(HttpContext context)
    {
        List<string> segments = new List<string>();
        bool flag = false;
        for (int i = 0; i < context.Request.Url.Segments.Length; i++)
        {
          string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
          if (flag) segments.Add(s);
          if (s == "upload") flag = true;
        }
       return segments.ToArray();
    }


    private void SaveFiles(HttpContext context, AttachmentPath.Folder folder)
    {
      StringBuilder builder = new StringBuilder();
      string path = AttachmentPath.GetPath(UserSession.LoginUser, UserSession.LoginUser.OrganizationID, folder);
      HttpFileCollection files = context.Request.Files;
      for (int i = 0; i < files.Count; i++)
			{
        if(files[i].ContentLength > 0)
        {
          string fileName = RemoveSpecialCharacters(DataUtils.VerifyUniqueUrlFileName(path, Path.GetFileName(files[i].FileName)));
          if (builder.Length > 0) builder.Append(",");
          builder.Append(fileName);
          fileName = Path.Combine(path, fileName); 
          files[i].SaveAs(fileName);
        }
			}
      context.Response.Write(builder.ToString());
    }

    private string RemoveSpecialCharacters(string text)
    {
      StringBuilder builder = new StringBuilder();
      foreach (char c in text)
	    {
        if (!char.IsLetterOrDigit(c) && c != '.') builder.Append("_");
        else builder.Append(c);
	    }
      return builder.ToString();
    
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
}
