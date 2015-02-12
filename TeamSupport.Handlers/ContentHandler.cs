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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using OfficeOpenXml;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

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

        int organizationID = -1;
        if (int.TryParse(segments[0], out organizationID))
        {
          switch (segments[1])
          {
            case "images":  ProcessImages(context, segments.ToArray(), organizationID); break;
            case "chat": ProcessChat(context, segments[2], organizationID); break;
            case "reports": ProcessReport(context, int.Parse(segments[2]), (context.Request["Type"] == null ? "old" : context.Request["Type"])); break;
            case "ticketexport": ProcessTicketExport(context); break;
            case "attachments": ProcessAttachment(context, int.Parse(segments[2])); break;
            case "avatar": ProcessAvatar(context, segments.ToArray(), organizationID); break;
            case "useravatar": ProcessUserAvatar(context, segments.ToArray(), organizationID); break;
            case "initialavatar": ProcessInitialAvatar(context, segments.ToArray(), organizationID); break;
            case "agentrating": ProcessRatingImages(context, segments.ToArray(), organizationID); break;
            case "productcustomers": ProcessProductCustomers(context, int.Parse(segments[2]), context.Request["Type"]); break;
            case "productversioncustomers": ProcessProductVersionCustomers(context, int.Parse(segments[2]), context.Request["Type"]); break;
            default: context.Response.End(); break;
          }
        }
        else
        {
          switch (segments[0])
          {
            case "reports": ProcessReport(context, int.Parse(segments[1]), (context.Request["Type"] == null ? "old" : context.Request["Type"])); break;
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

    private void ProcessRatingImages(HttpContext context, string[] segments, int organizationID)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 1; i < segments.Length; i++)
        {
            if(i != 1)
                builder.Append("\\");
            builder.Append(segments[i]);
        }
        string path = builder.ToString();
        string fileName = "";
        if (Path.GetExtension(path) == "")
        {
            path = Path.ChangeExtension(path, ".png");
            string imageFile = Path.GetFileName(path);
            path = Path.GetDirectoryName(path);
            string imagePath = AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.AgentRating);
            fileName = Path.Combine(imagePath, imageFile);
            if (!File.Exists(fileName))
            {
                imagePath = Path.Combine(AttachmentPath.GetDefaultPath(LoginUser.Anonymous, AttachmentPath.Folder.AgentRating), path);
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
      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

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

    private void ProcessUserAvatar(HttpContext context, string[] segments, int organizationID)
    {

      int userID = int.Parse(segments[2]);
      int size = int.Parse(segments[3]);
      string cacheFileName = "";
      string cachePath = Path.Combine(GetImageCachePath(), "Avatars\\" + organizationID.ToString());
      if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);
      
      cacheFileName = Path.Combine(cachePath, userID.ToString()+"-"+size.ToString()+".jpg");
      // found the last cache
      if (File.Exists(cacheFileName)) 
      {
        WriteImage(context, cacheFileName);
        return;
      }
      
      //New image, check if one has been uploaded
      string originalFileName = AttachmentPath.GetImageFileName(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.ProfileImages), userID.ToString() + "avatar");
      if (File.Exists(originalFileName))
      {
        // original image, resize, make circle, cache it
        using (Image image = Image.FromFile(originalFileName))
        using (Image scaledImage = ScaleImage(image, size, size))
        using (Image croppedImage = CropImage(scaledImage, size))
        {
          croppedImage.Save(cacheFileName, ImageFormat.Jpeg);
        }
        WriteImage(context, cacheFileName);
        return;
      }
      // no picture found, make a circle with first initial and cache it

      User user = Users.GetUser(LoginUser.Anonymous, userID);
      string initial = "A";

      if (!string.IsNullOrWhiteSpace(user.FirstName)) initial = user.FirstName.Substring(0, 1).ToUpper();
      else if (!string.IsNullOrWhiteSpace(user.LastName)) initial = user.LastName.Substring(0, 1).ToUpper();
      
      using (Image initialImage = MakeInitialSquare(initial, GetInitialColor(initial), size))
      {
        initialImage.Save(cacheFileName, ImageFormat.Jpeg);
      }
      WriteImage(context, cacheFileName);
      return;
      
    }

    private void ProcessInitialAvatar(HttpContext context, string[] segments, int organizationID)
    {

      string initial = segments[2].Substring(0, 1).ToUpper();
      int size = int.Parse(segments[3]);
      string cacheFileName = "";
      string cachePath = Path.Combine(GetImageCachePath(), "Initials");
      if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);


      cacheFileName = Path.Combine(cachePath, initial + "-" + size.ToString() + ".jpg");
      // found the last cache
      if (File.Exists(cacheFileName))
      {
        WriteImage(context, cacheFileName);
        return;
      }

      using (Image initialImage = MakeInitialSquare(initial, GetInitialColor(initial), size))
      {
        initialImage.Save(cacheFileName, ImageFormat.Jpeg);
      }
      WriteImage(context, cacheFileName);
      return;

    }
   
    private static Color GetInitialColor(string initial)
    {
      
      Dictionary<string, string> d = new Dictionary<string, string>()
      {
        { "A", "F44336" },
        { "B", "E91E63" },
        { "C", "BA55D3" },
        { "D", "9C27B0" },
        { "E", "673AB7" },
        { "F", "3F51B5" },
        { "G", "3276B1" },
        { "H", "03A9F4" },
        { "I", "00BCD4" },
        { "J", "009688" },
        { "K", "4CAF50" },
        { "L", "8BC34A" },
        { "M", "CDDC39" },
        { "N", "FFEB3B" },
        { "O", "FFC107" },
        { "P", "FF9800" },
        { "Q", "FF5722" },
        { "R", "795548" },
        { "S", "607D8B" },
        { "T", "F44336" },
        { "U", "E91E63" },
        { "V", "BA55D3" },
        { "W", "9C27B0" },
        { "X", "673AB7" },
        { "Y", "3F51B5" },
        { "Z", "3276B1" },
        { "0", "03A9F4" },
        { "1", "00BCD4" },
        { "2", "009688" },
        { "3", "4CAF50" },
        { "4", "8BC34A" },
        { "5", "CDDC39" },
        { "6", "FFEB3B" },
        { "7", "FFC107" },
        { "8", "FF9800" },
        { "9", "FF5722" }
      };


      string color = d.ContainsKey(initial) ? d[initial] : "999999";
      return ColorTranslator.FromHtml("#" + color);
          
    }

    public static Image MakeInitialSquare(string initial, Color color, int size)
    {
      Bitmap bmp = new Bitmap(size, size, PixelFormat.Format32bppPArgb);
      GraphicsPath gp = new GraphicsPath();
      using (Graphics gr = Graphics.FromImage(bmp))
      {
        gr.Clear(Color.Transparent);
        using (gp)
        {
          using (gr)
          {
            //using (Pen pen = new Pen(color, 3))
            using (Brush brush = new SolidBrush(color))
            {
              gr.FillRectangle(brush, 0, 0, size, size);
              gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
              StringFormat strFormat = new StringFormat();
              strFormat.Alignment = StringAlignment.Center;
              strFormat.LineAlignment = StringAlignment.Center;
              System.Drawing.FontFamily fontFamily;
              try
              {
                fontFamily = new System.Drawing.FontFamily("Roboto");
              }
              catch (Exception)
              {
                fontFamily = new System.Drawing.FontFamily("Arial");
              }

              int fontSize = 8;
              Font font = null;
              double maxSize = size * 0.7;
              while (true)
              {
                font = new Font(fontFamily, fontSize);
                SizeF sizeF = gr.MeasureString("X", font);
                if ((sizeF.Height > maxSize && sizeF.Width > maxSize) || fontSize > 100)
                {
                  font = new Font(fontFamily, fontSize - 1);
                  break;
                }
                fontSize++;
              }
              gr.DrawString(initial, font, Brushes.White, new Rectangle(0, (int)(size * 0.06), size, size), strFormat);
            }
          }
        }
      }
      return bmp;
    }

    public static Image MakeInitialCircle(string initial, Color color, int size)
    {
      Bitmap bmp = new Bitmap(size, size, PixelFormat.Format32bppPArgb);
      GraphicsPath gp = new GraphicsPath();
      using (Graphics gr = Graphics.FromImage(bmp))
      {
        gr.Clear(Color.Transparent);
        using (gp)
        {
          using (gr)
          {
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(color, 3))
            using (Brush brush = new SolidBrush(color))
            {
              gr.DrawEllipse(pen, 1, 1, size-3, size-3);
              gr.FillEllipse(brush, 1,1,size-3, size-3);
              gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
              StringFormat strFormat = new StringFormat();
              strFormat.Alignment = StringAlignment.Center;
              strFormat.LineAlignment = StringAlignment.Center;
              gr.DrawString(initial, new Font("Arial", 28), Brushes.White, new Rectangle(0, 3, size, size), strFormat);
            }
          }
        }
      }
      return bmp;
    }

    public static Image MakeRound(Image img)
    {
      Bitmap bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb);
      GraphicsPath gp = new GraphicsPath();
      using (Graphics gr = Graphics.FromImage(bmp))
      {
        gr.Clear(Color.Transparent);
        using (gp)
        {
          gp.AddEllipse(0, 0, img.Width, img.Height);
          using (gr)
          {
            gr.SetClip(gp);
            gr.DrawImage(img, Point.Empty);
            Pen pen = new Pen(Color.FromArgb(255, 255, 255, 255), 3);
            gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gr.CompositingQuality = CompositingQuality.HighQuality;
            gr.SmoothingMode = SmoothingMode.AntiAlias;
            gr.DrawEllipse(pen, 0, 0, img.Width, img.Height);
          }
        }
      }
      return bmp;
    }

    public static Image CropImage(Image image, int size)
    {
      Bitmap newImage = new Bitmap(size, size);
      Rectangle cropRect = image.Width > image.Height ? new Rectangle((int)((image.Width - size) / 2), 0, size, size) : new Rectangle(0, 0, size, size);

      using (Graphics g = Graphics.FromImage(newImage))
      {
        g.DrawImage(image, new Rectangle(0, 0, newImage.Width, newImage.Height),
                         cropRect,
                         GraphicsUnit.Pixel);
      }
      return newImage;
    }

    public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
    {
      var ratioX = (double)maxWidth / image.Width;
      var ratioY = (double)maxHeight / image.Height;
      var ratio = Math.Max(ratioX, ratioY);

      var newWidth = (int)(image.Width * ratio);
      var newHeight = (int)(image.Height * ratio);
      var newImage = new Bitmap(newWidth, newHeight);
      Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
      return newImage;
    }

    private static string GetImageCachePath()
    {
      string result = System.Web.Configuration.WebConfigurationManager.AppSettings["ImageCachePath"];
      return result == null ? "C:\\TSCache" : result;
    
    }

    private void ProcessAttachment(HttpContext context, int attachmentID)
    {
      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

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
      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      
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
      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

      //http://127.0.0.1/tsdev/dc/1078/reports/7401
      Report report = Reports.GetReport(TSAuthentication.GetLoginUser(), reportID, TSAuthentication.UserID);

      if (report == null ||  (report.OrganizationID != TSAuthentication.GetLoginUser().OrganizationID && report.OrganizationID != null))
      {
        context.Response.Write("Unauthorized");
        context.Response.ContentType = "text/html";
        return;
      }

      dynamic settings = null;
      string sortField = null;
      bool isDesc = true;
      try
      {
        object o = report.Row["Settings"];
        if (o != null && o != DBNull.Value)
        {
          settings = JObject.Parse(o.ToString());
          if (settings["SortField"] != null) sortField = settings["SortField"].ToString();
          if (settings["IsSortAsc"] != null) isDesc = (bool)settings["IsSortAsc"] == false;

        }
      }
      catch (Exception)
      {
      }

      DataTable table = Reports.GetReportTable(report.Collection.LoginUser, report.ReportID, 0, 10000000, sortField, isDesc, true, false);

      try
      {
        if (settings != null)
        {
          if (settings["Columns"] != null)
          {
            List<string> columnNames = new List<string>();
            for (int i = 0; i < settings["Columns"].Count; i++)
            {
              columnNames.Add(settings["Columns"][i]["id"].ToString());
            }

            int index = 0;
            foreach (string columnName in columnNames)
            {
              DataColumn column = table.Columns[columnName];
              if (column != null)
              {
                column.SetOrdinal(index);
                index++;
              }
            }
          }
        }

      }
      catch (Exception)
      {

      }


      if (type.ToLower() == "excel")
      {
        using (ExcelPackage pck = new ExcelPackage())
        {
          ExcelWorksheet ws = pck.Workbook.Worksheets.Add(report.Name);

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
              r = ws.Cells[2, i + 1, rowCount + 1, i + 1];
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
        string text = DataUtils.TableToCsv(report.Collection.LoginUser, table);
        context.Response.Write(text);
        context.Response.ContentType = "application/octet-stream";
        context.Response.AddHeader("content-disposition", "attachment; filename=\"" + report.Name + ".csv\"");

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
      DateTime lastWriteDate = File.GetLastWriteTimeUtc(fileName);
      try
      {
        if (context.Request.Headers["If-Modified-Since"] != null && lastWriteDate.Subtract(DateTime.Parse(context.Request.Headers["If-Modified-Since"]).ToUniversalTime()).TotalSeconds < 5)
        {
          context.Response.StatusCode = 304;
          context.Response.SuppressContent = true;
          context.Response.End();
          return;
        }
      }
      catch (Exception ex)
      {
        
      }

      string ext = Path.GetExtension(fileName).ToLower().Substring(1);
      using (Image image = new Bitmap(fileName))
      {
        context.Response.ContentType = "image/" + ext;
        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        context.Response.Cache.SetExpires(DateTime.Now.AddHours(8));
        context.Response.Cache.SetMaxAge(new TimeSpan(8, 0, 0));
        context.Response.Headers["Last-Modified"] = lastWriteDate.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'");
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
  
    private void ProcessProductCustomers(HttpContext context, int productID, string type)
    {
      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      
      if (productID > 0)
      {
        LoginUser loginUser = TSAuthentication.GetLoginUser();
        Products products = new Products(loginUser);
        products.LoadByProductID(productID);
        OrganizationProductsView organizationProductsView = new OrganizationProductsView(loginUser);
        organizationProductsView.LoadByProductIDForExport(productID);

        if (type.ToLower() == "excel")
        {
          using (ExcelPackage pck = new ExcelPackage())
          {
            //Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(products[0].Name + " product customers");

            //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
            ws.Cells["A1"].LoadFromDataTable(organizationProductsView.Table, true);

            int columnCount = organizationProductsView.Table.Columns.Count;
            int rowCount = organizationProductsView.Table.Rows.Count;

            ExcelRange r;

            System.Globalization.DateTimeFormatInfo dtfi = loginUser.CultureInfo.DateTimeFormat;

            // which columns have dates in
            for (int i = 0; i < columnCount; i++)
            {
              // if cell header value matches a date column
              if (organizationProductsView.Table.Columns[i].DataType == typeof(System.DateTime))
              {
                r = ws.Cells[2, i + 1, rowCount + 1, i + 1];
                r.AutoFitColumns();
                r.Style.Numberformat.Format = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;
              }
            }
            // get all data and autofit
            r = ws.Cells[1, 1, rowCount + 1, columnCount];
            r.AutoFitColumns();

            //Write it back to the client
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.Response.AddHeader("content-disposition", "attachment;  filename=" + products[0].Name + " product customers.xlsx");
            context.Response.BinaryWrite(pck.GetAsByteArray());
          }
        }
        else
        {
          string text = DataUtils.TableToCsv(loginUser, organizationProductsView.Table);
          context.Response.Write(text);
          context.Response.ContentType = "application/octet-stream";
          context.Response.AddHeader("content-disposition", "attachment; filename=\"" + products[0].Name + " product customers.csv\"");

        }
      }
    }

    private void ProcessProductVersionCustomers(HttpContext context, int productVersionID, string type)
    {
      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

      if (productVersionID > 0)
      {
        LoginUser loginUser = TSAuthentication.GetLoginUser();
        ProductVersions productVersions = new ProductVersions(loginUser);
        productVersions.LoadByProductVersionID(productVersionID);
        OrganizationProductsView organizationProductsView = new OrganizationProductsView(loginUser);
        organizationProductsView.LoadByProductVersionIDForExport(productVersionID);

        if (type.ToLower() == "excel")
        {
          using (ExcelPackage pck = new ExcelPackage())
          {
            //Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(productVersions[0].VersionNumber + " product version customers");

            //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
            ws.Cells["A1"].LoadFromDataTable(organizationProductsView.Table, true);

            int columnCount = organizationProductsView.Table.Columns.Count;
            int rowCount = organizationProductsView.Table.Rows.Count;

            ExcelRange r;

            System.Globalization.DateTimeFormatInfo dtfi = loginUser.CultureInfo.DateTimeFormat;

            // which columns have dates in
            for (int i = 0; i < columnCount; i++)
            {
              // if cell header value matches a date column
              if (organizationProductsView.Table.Columns[i].DataType == typeof(System.DateTime))
              {
                r = ws.Cells[2, i + 1, rowCount + 1, i + 1];
                r.AutoFitColumns();
                r.Style.Numberformat.Format = dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;
              }
            }
            // get all data and autofit
            r = ws.Cells[1, 1, rowCount + 1, columnCount];
            r.AutoFitColumns();

            //Write it back to the client
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.Response.AddHeader("content-disposition", "attachment;  filename=" + productVersions[0].VersionNumber + " product version customers.xlsx");
            context.Response.BinaryWrite(pck.GetAsByteArray());
          }
        }
        else
        {
          string text = DataUtils.TableToCsv(loginUser, organizationProductsView.Table);
          context.Response.Write(text);
          context.Response.ContentType = "application/octet-stream";
          context.Response.AddHeader("content-disposition", "attachment; filename=\"" + productVersions[0].VersionNumber + " product version customers.csv\"");

        }
      }
    }
  }
}
