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
using TeamSupport.WebUtils;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Web.Security;
using OfficeOpenXml;

namespace TeamSupport.Handlers
{
  public class ContentHandler : IHttpHandler
  {
    #region IHttpHandler Members

    public bool IsReusable
    {
      get { return false; }
    }


    public void ProcessRequest(HttpContext context)
    {
      try
      {
        string connection = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;

        List<string> segments = new List<string>();
        bool flag = false;
        for (int i = 0; i < context.Request.Url.Segments.Length; i++)
        {
          string s = context.Request.Url.Segments[i].ToLower().Trim().Replace("/", "");
          if (flag) segments.Add(s);
          if (s == "dc") flag = true;
        }

        System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
        if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        int organizationID = -1;
        if (int.TryParse(segments[0], out organizationID))
        {
          switch (segments[1])
          {
            case "images": ProcessImages(context, segments.ToArray(), organizationID); break;
            case "chat": ProcessChat(context, segments[2], organizationID); break;
            case "reports": ProcessReport(context, int.Parse(segments[2]), (context.Request["Type"] == null ? "CSV" : context.Request["Type"])); break;
            case "ticketexport": ProcessTicketExport(context); break;
            case "attachments": ProcessAttachment(context, int.Parse(segments[2])); break;
            case "avatar": ProcessAvatar(context, segments.ToArray(), organizationID); break;
            default: context.Response.End(); break;
          }
        }
        else
        {
          switch (segments[0])
          {
            case "reports": ProcessReport(context, int.Parse(segments[1]), (context.Request["Type"] == null ? "CSV" : context.Request["Type"])); break;
            case "ticketexport": ProcessTicketExport(context); break;
            case "attachments": ProcessAttachment(context, int.Parse(segments[1])); break;
            default: context.Response.End(); break;
          }
        }

      }
      catch (Exception ex)
      {
        context.Response.ContentType = "text/html";
        context.Response.Write(ex.Message + "<br />" + ex.StackTrace);
      }
      context.Response.End();
    }


    #endregion

    private void ProcessImages(HttpContext context, string[] segments, int organizationID)
    {
      StringBuilder builder = new StringBuilder();
      for (int i = 2; i < segments.Length; i++)
      {
        if (i != 2) builder.Append("\\");
        builder.Append(segments[i]);
      }
      string path = builder.ToString();
      string fileName = "";
      if (Path.GetExtension(path) == "")
      {
        path = Path.ChangeExtension(path, ".jpg");
        string imageFile = Path.GetFileName(path);
        path = Path.GetDirectoryName(path);
        string imagePath = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.Images), path);
        fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
        if (!File.Exists(fileName))
        {
          imagePath = Path.Combine(AttachmentPath.GetDefaultPath(LoginUser.Anonymous, AttachmentPath.Folder.Images), path);
          fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
        }

      }
      else
      {
        fileName = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.Images), path);
      }
      if (File.Exists(fileName)) WriteImage(context, fileName);
    }

    private void ProcessChat(HttpContext context, string command, int organizationID)
    {
      if (command == "image")
      {
        bool isAvailable = ChatRequests.IsOperatorAvailable(LoginUser.Anonymous, organizationID);
        string fileName = isAvailable ? "chat_available" : "chat_unavailable";
        fileName = AttachmentPath.FindImageFileName(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.ChatImages, fileName);
        WriteImage(context, fileName);
      }
      else if (command == "logo")
      {
        string fileName = AttachmentPath.FindImageFileName(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.ChatImages, "chat_logo");
        WriteImage(context, fileName);
      }
    }

    private void ProcessAvatar(HttpContext context, string[] segments, int organizationID)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 2; i < segments.Length; i++)
        {
            if (i != 2) builder.Append("\\");
            builder.Append(segments[i]);
        }
        string path = builder.ToString();

        Attachment attachment = Attachments.GetAttachment(LoginUser.Anonymous, Int32.Parse(path));

        path = attachment.FileName;

        string fileName = "";
        if (Path.GetExtension(path) == "")
        {
            path = Path.ChangeExtension(path, ".jpg");
            string imageFile = Path.GetFileName(path);
            path = Path.GetDirectoryName(path);
            string imagePath = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.ProfileImages), path);
            fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
            if (!File.Exists(fileName))
            {
                imagePath = Path.Combine(AttachmentPath.GetDefaultPath(LoginUser.Anonymous, AttachmentPath.Folder.ProfileImages), path);
                fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
            }

        }
        else
        {
            fileName = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.ProfileImages), path);
        }
        if (File.Exists(fileName)) WriteImage(context, fileName);
    }

    private void ProcessAttachment(HttpContext context, int attachmentID)
    {

      //http://127.0.0.1/tsdev/dc/attachments/7401
      //https://app.teamsupport.com/dc/attachments/{AttachmentID}
      Attachment attachment = Attachments.GetAttachment(LoginUser.Anonymous, attachmentID);
      Organization organization = Organizations.GetOrganization(attachment.Collection.LoginUser, attachment.OrganizationID);
      User user = null;
      bool isAuthenticated = attachment.OrganizationID == TSAuthentication.OrganizationID;


      if (isAuthenticated)
      {
        user = Users.GetUser(attachment.Collection.LoginUser, TSAuthentication.UserID);
      }
      else
      {
        try
        {
          FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(context.Request.Cookies["Portal_Session"].Value);
          int userID = int.Parse(authTicket.UserData.Split('|')[0]);
          user = Users.GetUser(attachment.Collection.LoginUser, userID);


          if (attachment.RefType == ReferenceType.Actions)
          {
            TeamSupport.Data.Action action = Actions.GetAction(attachment.Collection.LoginUser, attachment.RefID);
            if (action.IsVisibleOnPortal)
            {
              Ticket ticket = Tickets.GetTicket(action.Collection.LoginUser, action.TicketID);
              if (ticket.IsVisibleOnPortal)
              {
                Organizations organizations = new Organizations(attachment.Collection.LoginUser);
                organizations.LoadByTicketID(ticket.TicketID);
                isAuthenticated = organizations.FindByOrganizationID(user.OrganizationID) != null;
              }
            }
          }

        }
        catch (Exception)
        {
        }
      }

      isAuthenticated = isAuthenticated || organization.AllowUnsecureAttachmentViewing;        


      if (!isAuthenticated)
      {
        context.Response.Write("Unauthorized");
        context.Response.ContentType = "text/html";
        return;
      }

      if (!File.Exists(attachment.Path))
      {
        context.Response.Write("Invalid attachment.");
        context.Response.ContentType = "text/html";
        return;
      }

      /*
      AttachmentDownload download = (new AttachmentDownloads(attachment.Collection.LoginUser)).AddNewAttachmentDownload();
      download.AttachmentID = attachment.AttachmentID;
      download.UserID = user.UserID;
      download.DateDownloaded = DateTime.UtcNow;
      download.Collection.Save();
       */
      


      string openType = "inline";
      string fileType = attachment.FileType;

      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser == "IE")
      {
        if (attachment.FileType.ToLower().IndexOf("audio") > -1)
        {
          openType = "attachment";
        }
        else if (attachment.FileType.ToLower().IndexOf("-zip") > -1 ||
                 attachment.FileType.ToLower().IndexOf("/zip") > -1 ||
                 attachment.FileType.ToLower().IndexOf("zip-") > -1)
        {
          fileType = "application/octet-stream";
        }
      }
      
      context.Response.AddHeader("Content-Disposition", openType + "; filename=\"" + attachment.FileName + "\"");
      context.Response.AddHeader("Content-Length", attachment.FileSize.ToString());
      context.Response.ContentType = fileType;
      context.Response.WriteFile(attachment.Path);
    }

    private void ProcessTicketExport(HttpContext context)
    {
      string value = context.Request.QueryString["filter"];
      try
      {
        TicketLoadFilter filter = JsonConvert.DeserializeObject<TicketLoadFilter>(value);
        SqlCommand command = TicketsView.GetLoadExportCommand(TSAuthentication.GetLoginUser(), filter);

        string text = DataUtils.CommandToCsv(TSAuthentication.GetLoginUser(), command, false);
        /*
        MemoryStream stream = new MemoryStream();
        ZipOutputStream zip = new ZipOutputStream(stream);
        zip.SetLevel(9);
        zip.PutNextEntry(new ZipEntry("TicketExport.csv"));
        Byte[] bytes = Encoding.UTF8.GetBytes(text);
        zip.Write(bytes, 0, bytes.Length);
        zip.CloseEntry();
        zip.Finish();
        stream.WriteTo(context.Response.OutputStream);
        //context.Response.ContentType = "application/x-zip-compressed";
        context.Response.ContentType = "application/octet-stream";
        context.Response.AddHeader("Content-Disposition", "attachment; filename=\"TicketExport.zip\"");
        context.Response.AddHeader("Content-Length", stream.Length.ToString());
        zip.Close();*/

        context.Response.Write(text);
        context.Response.ContentType = "application/octet-stream";
        context.Response.AddHeader("Content-Disposition", "attachment; filename=\"TicketExport.csv\"");
      }
      catch (Exception ex)
      {
        context.Response.Write("Error retrieving tickets. <br/><br/>" + ex.Message);
        context.Response.ContentType = "text/html";
        ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Ticket Grid Export");
        return;
      }
    }

    private void ProcessReport(HttpContext context, int reportID, string type)
    {

      //http://127.0.0.1/tsdev/dc/1078/reports/7401
      Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID);

      if (report == null ||  (report.OrganizationID != TSAuthentication.GetLoginUser().OrganizationID && report.OrganizationID != null))
      {
        context.Response.Write("Unauthorized");
        context.Response.ContentType = "text/html";
        return;
      }
     

      if (false)
      {
        string sql = report.GetSqlOld(false);
        SqlCommand command = new SqlCommand(sql);
        Report.CreateParameters(TSAuthentication.GetLoginUser(), command, TSAuthentication.GetLoginUser().UserID);
        string text = DataUtils.CommandToCsv(TSAuthentication.GetLoginUser(), command, false);

        context.Response.Write(text);
        context.Response.ContentType = "application/octet-stream";
        context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + report.Name + ".csv\"");
        //context.Response.AddHeader("Content-Length", text.Length.ToString());
      }
      else
      {
        if (type.ToLower() == "excel")
        {
          using (ExcelPackage pck = new ExcelPackage())
          {
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(report.Name);
            
            DataTable table = Reports.GetReportTable(report.Collection.LoginUser, report.ReportID, 0, 10000000, null, true, true, false);
            System.Globalization.DateTimeFormatInfo dtfi = TSAuthentication.GetLoginUser().CultureInfo.DateTimeFormat;
            //ws.Column(0).Style.Numberformat.Format = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;
            ws.Cells["A1"].LoadFromDataTable(table, true);
            ws.Cells["A1:" + GetExcelColumnName(table.Columns.Count) + "1"].AutoFilter = true;

            int columnCount = table.Columns.Count;
            int rowCount = table.Rows.Count;

            ExcelRange r;

            // which columns have dates in
            for (int i = 0; i < columnCount; i++)
            {
              // if cell header value matches a date column
              if (table.Columns[i].DataType == typeof(System.DateTime))
              {
                r = ws.Cells[2, i+1, rowCount + 1, i+1];
                r.AutoFitColumns();
                r.Style.Numberformat.Format = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern; 
              }
            }
            // get all data and autofit
            r = ws.Cells[1, 1, rowCount + 1, columnCount];
            r.AutoFitColumns();
 
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.Response.AddHeader("content-disposition", "attachment;  filename=\"" + report.Name + ".xlsx\"");
            context.Response.BinaryWrite(pck.GetAsByteArray());
          }

        }
        else
        {
          DataTable table = Reports.GetReportTable(report.Collection.LoginUser, report.ReportID, 0, 10000000, null, true, true, false);
          string text = DataUtils.TableToCsv(report.Collection.LoginUser, table);
          context.Response.Write(text);
          context.Response.ContentType = "application/octet-stream";
          context.Response.AddHeader("content-disposition", "attachment; filename=\"" + report.Name + ".csv\"");

        }

      }
    }

    private string GetExcelColumnName(int columnNumber)
    {
      int dividend = columnNumber;
      string columnName = String.Empty;
      int modulo;

      while (dividend > 0)
      {
        modulo = (dividend - 1) % 26;
        columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
        dividend = (int)((dividend - modulo) / 26);
      }

      return columnName;
    }

    private void WriteImage(HttpContext context, string fileName)
    {
      string ext = Path.GetExtension(fileName).ToLower().Substring(1);
      using (Image image = new Bitmap(fileName))
      {
        context.Response.ContentType = "image/" + ext;
        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        context.Response.Cache.SetExpires(DateTime.Now.AddHours(8));
        context.Response.Cache.SetMaxAge(new TimeSpan(8, 0, 0));
        System.Drawing.Imaging.ImageFormat format;
        switch (ext)
        {
          case "png": format = System.Drawing.Imaging.ImageFormat.Png; break;
          case "gif": format = System.Drawing.Imaging.ImageFormat.Gif; break;
          case "jpg": format = System.Drawing.Imaging.ImageFormat.Jpeg; break;
          case "bmp": format = System.Drawing.Imaging.ImageFormat.Bmp; break;
          case "ico": format = System.Drawing.Imaging.ImageFormat.Icon; break;
          default: format = System.Drawing.Imaging.ImageFormat.Jpeg; break;
        }
        //image.Save(context.Response.OutputStream, format);
        MemoryStream stream = new MemoryStream();
        image.Save(stream, format);
        stream.WriteTo(context.Response.OutputStream);
      }


    }
  }
}
