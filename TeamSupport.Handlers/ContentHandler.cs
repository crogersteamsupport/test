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
using DDay.iCal.Serialization;
using DDay.iCal;
using DDay.Collections;
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
            case "attachments": ProcessAttachment(context, segments[2]); break;
            case "avatar": ProcessAvatar(context, segments.ToArray(), organizationID); break;
            case "useravatar": ProcessUserAvatar(context, segments.ToArray(), organizationID); break;
            case "initialavatar": ProcessInitialAvatar(context, segments.ToArray(), organizationID); break;
            case "agentrating": ProcessRatingImages(context, segments.ToArray(), organizationID); break;
            case "productcustomers": ProcessProductCustomers(context, int.Parse(segments[2]), context.Request["Type"]); break;
            case "productversioncustomers": ProcessProductVersionCustomers(context, int.Parse(segments[2]), context.Request["Type"]); break;
            case "calendarfeed": ProcessCalendarFeed(context, segments.ToArray(), organizationID); break;
            case "companylogo": ProcessCompanyLogo(context, segments.ToArray(), organizationID); break;
            case "contactavatar": ProcessContactAvatar(context, segments.ToArray(), organizationID); break;
            case "importlog": ProcessImportLog(context, int.Parse(segments[2])); break;
            default: context.Response.End(); break;
          }
        }
        else
        {
          switch (segments[0])
          {
            case "reports": ProcessReport(context, int.Parse(segments[1]), (context.Request["Type"] == null ? "old" : context.Request["Type"])); break;
            case "ticketexport": ProcessTicketExport(context); break;
            case "attachments": ProcessAttachment(context, segments[1]); break;
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

    /* USER: UserAvatar instead. This is old style.  It will be going away */
    private void ProcessAvatar(HttpContext context, string[] segments, int organizationID)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 2; i < segments.Length; i++)
        {
            if (i != 2) builder.Append("\\");
            builder.Append(segments[i]);
        }
        string path = builder.ToString();

        TeamSupport.Data.Attachment attachment = Attachments.GetAttachment(LoginUser.Anonymous, Int32.Parse(path));

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

    private void ProcessCalendarFeed(HttpContext context, string[] segments, int organizationID)
    {
      DDay.iCal.iCalendar iCal = new DDay.iCal.iCalendar();
      string guid = segments[2];

      Users u = new Users(LoginUser.Anonymous);
      u.LoadByCalGUID(guid);

      if (u.Count > 0)
      {
        if (u[0].TimeZoneID != null)
        {
          System.TimeZoneInfo timezoneinfo = System.TimeZoneInfo.FindSystemTimeZoneById(u[0].TimeZoneID);
          iCalTimeZone timezone = iCalTimeZone.FromSystemTimeZone(timezoneinfo);
          iCal.AddTimeZone(timezone);
          iCal.AddChild(timezone);
        }
        else
        {
          iCal.AddLocalTimeZone();
        }




        TeamSupport.Data.CalendarEvents events = new CalendarEvents(LoginUser.Anonymous);
        events.LoadAll(organizationID, u[0].UserID);

        foreach (CalendarEvent calevent in events)
        {
          Event evt = iCal.Create<Event>();
          evt.Summary = calevent.Title;
          evt.Description = calevent.Description;
          evt.IsAllDay = calevent.AllDay;

          if (calevent.AllDay)
          {
            evt.Start = (iCalDateTime)calevent.StartDateUtc.Date;
            DateTime dt = (DateTime)calevent.EndDateUtc;
            evt.End = (iCalDateTime)dt.Date;
          }
          else
          {
            evt.Start = (iCalDateTime)calevent.StartDateUtc;
            evt.End = (iCalDateTime)calevent.EndDateUtc;
          }

        }

        Tickets tickets = new Tickets(LoginUser.Anonymous);
        tickets.LoadAllDueDates(u[0].UserID, u[0].OrganizationID);
        foreach (Ticket ticket in tickets)
        {
          Event evt = iCal.Create<Event>();
          evt.Summary = ticket.Name;
          evt.Start = (iCalDateTime)ticket.DueDate;
        }

        Reminders reminders = new Reminders(LoginUser.Anonymous);
        reminders.LoadByUser(u[0].UserID);
        foreach (Reminder reminder in reminders)
        {
          Event evt = iCal.Create<Event>();
          switch (reminder.RefType)
          {
            case ReferenceType.Tickets:
              Ticket t = Tickets.GetTicket(LoginUser.Anonymous, reminder.RefID);
              if (t != null)
                evt.Summary = "Ticket Reminder: " + t.Name;
              break;
            case ReferenceType.Organizations:
              Organization o = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), reminder.RefID);
              if (o != null)
                evt.Summary = "Company Reminder: " + o.Name;
              break;
            case ReferenceType.Contacts:
              User usr = Users.GetUser(TSAuthentication.GetLoginUser(), reminder.RefID);
              if (usr != null)
                evt.Summary = "Contact Reminder: " + usr.FirstLastName;
              break;
          }
          evt.Start = (iCalDateTime)reminder.DueDate;
        }


        //Event evt = iCal.Create<Event>();
        //// Set information about the event
        //evt.Start = iCalDateTime.Today.AddHours(8);
        //evt.End = evt.Start.AddHours(18); // This also sets the duration
        //evt.Description = "The event description";
        //evt.Location = "Event location";
        //evt.Summary = "18 hour event summary";

        //// Set information about the second event
        //evt = iCal.Create<Event>();
        //evt.Start = iCalDateTime.Today.AddDays(5);
        //evt.End = evt.Start.AddDays(1);
        //evt.IsAllDay = true;
        //evt.Summary = "All-day event";

        //// Set information about the second event
        //evt = iCal.Create<Event>();
        //evt.Start = iCalDateTime.Today.AddHours(13);
        //evt.End = evt.Start.AddHours(3);
        //evt.IsAllDay = true;
        //evt.Summary = "Erics newer new event";

        // Create a serialization context and serializer factory.
        // These will be used to build the serializer for our object.
        ISerializationContext ctx = new SerializationContext();
        ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
        // Get a serializer for our object
        IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;
      }


      var res = new DDay.iCal.Serialization.iCalendar.iCalendarSerializer().SerializeToString(iCal);
      using (var file = new System.IO.StreamWriter(Path.GetTempPath() + "out.ics"))
      {
        file.Write(res);
      }

      context.Response.Write(res);
      return;

      //int userID = int.Parse(segments[2]);

      //context.Response.Write(userID);

      //return;

    }

    private void ProcessUserAvatar(HttpContext context, string[] segments, int organizationID)
    {

      int userID = int.Parse(segments[2]);
      int size = int.Parse(segments[3]);
      string cacheFileName = "";
      string cachePath = Path.Combine(AttachmentPath.GetImageCachePath(LoginUser.Anonymous), "Avatars\\" + organizationID.ToString());
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

      string initial = segments[2].ToUpper();
      int size = int.Parse(segments[3]);
      string cacheFileName = "";
      string cachePath = Path.Combine(AttachmentPath.GetImageCachePath(LoginUser.Anonymous), "Initials");
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

    //https://app.teamsupport.com/dc/{OrganizationID}/CompanyLogo/{orgIdLogo}/{Size}/{page}
    private void ProcessCompanyLogo(HttpContext context, string[] segments, int organizationID)
    {
      int     logoOrganizationId  = int.Parse(segments[2]);
      int     size                = int.Parse(segments[3]);
      string  type                = segments.Length == 5 ? segments[4] : string.Empty;
      string  cacheFileName       = "";
      string cachePath = Path.Combine(AttachmentPath.GetImageCachePath(LoginUser.Anonymous), "CompanyLogo\\" + organizationID.ToString());
      bool    isIndexPage         = !string.IsNullOrEmpty(type) && type.ToLower() == "index";

      if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);

      cacheFileName = Path.Combine(cachePath, string.Format("{0}-{1}{2}.jpg", logoOrganizationId.ToString(), size.ToString(), string.IsNullOrEmpty(type) ? "" : "-" + type));

      // found the last cache
      if (File.Exists(cacheFileName))
      {
        WriteImage(context, cacheFileName);
        return;
      }

      //New image, check if one has been uploaded
      string originalFileName = AttachmentPath.GetImageFileName(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.OrganizationsLogo), logoOrganizationId.ToString());

      if (File.Exists(originalFileName))
      {
        // original image, resize, cache it
        using (Image image = Image.FromFile(originalFileName))
        using (Image scaledImage = ScaleImage(image, size, size))
        {
          //If coming from the index/search page crop it
          if (isIndexPage)
          {
            using (Image croppedImage = CropImage(scaledImage, size))
            {
              croppedImage.Save(cacheFileName, ImageFormat.Jpeg);
            }
          }
          else
          {
            scaledImage.Save(cacheFileName, ImageFormat.Jpeg);
          }
          
        }
        WriteImage(context, cacheFileName);
        return;
      }

      // no picture found, if it is for the index/search page then make a square with first initial and cache it. It'll be circled with css.
      if (isIndexPage)
      {
        Organization organization = Organizations.GetOrganization(LoginUser.Anonymous, logoOrganizationId);
        string initial = "A";

        if (organization != null && !string.IsNullOrWhiteSpace(organization.Name)) initial = organization.Name.Substring(0, 1).ToUpper();

        Color initialColor = ColorTranslator.FromHtml("#4CAF50");

        using (Image initialImage = MakeInitialSquare(initial, initialColor, size))
        {
          initialImage.Save(cacheFileName, ImageFormat.Jpeg);
        }

        WriteImage(context, cacheFileName);
      }

      return;
    }

    //https://app.teamsupport.com/dc/{OrganizationID}/contactavatar/{userId}/{Size}/{page}
    private void ProcessContactAvatar(HttpContext context, string[] segments, int organizationID)
    {
      int     organizationParentId  = (int)Organizations.GetOrganization(LoginUser.Anonymous, organizationID).ParentID;
      int     userId                = int.Parse(segments[2]);
      int     size                  = int.Parse(segments[3]);
      string  type                  = segments.Length == 5 ? segments[4] : string.Empty;
      string  cacheFileName         = "";
      string cachePath = Path.Combine(AttachmentPath.GetImageCachePath(LoginUser.Anonymous), "Avatars\\" + organizationParentId.ToString() + "\\Contacts\\");
      bool    isIndexPage           = !string.IsNullOrEmpty(type) && type.ToLower() == "index";

      if (!Directory.Exists(cachePath)) Directory.CreateDirectory(cachePath);

      cacheFileName = Path.Combine(cachePath, string.Format("{0}-{1}{2}.jpg", userId.ToString(), size.ToString(), string.IsNullOrEmpty(type) ? "" : "-" + type));

      // found the last cache
      if (File.Exists(cacheFileName))
      {
        WriteImage(context, cacheFileName);
        return;
      }

      //New image, check if one has been uploaded
      string originalFileName = AttachmentPath.GetImageFileName(AttachmentPath.GetPath(LoginUser.Anonymous, organizationParentId, AttachmentPath.Folder.ContactImages), userId.ToString() + "avatar");

      if (File.Exists(originalFileName))
      {
        // original image, resize, crop, cache it. It'll be circled with css.
        using (Image image = Image.FromFile(originalFileName))
        using (Image scaledImage = ScaleImage(image, size, size))
        using (Image croppedImage = CropImage(scaledImage, size))
        {
          croppedImage.Save(cacheFileName, ImageFormat.Jpeg);
        }
        WriteImage(context, cacheFileName);
        return;
      }

      // no picture found, make a square with first initial and cache it. It'll be circled with css.
      User user = Users.GetUser(LoginUser.Anonymous, userId);
      string initial = "A";

      if (user != null && !string.IsNullOrWhiteSpace(user.FirstName)) initial = user.FirstName.Substring(0, 1).ToUpper();

      using (Image initialImage = MakeInitialSquare(initial, !isIndexPage ? GetInitialColor(initial) : ColorTranslator.FromHtml("#FF9800"), size))
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
        { "B", "3276B1" },
        { "C", "4CAF50" },
        { "D", "FFC107" },
        { "E", "FF9800" },
        { "F", "F44336" },
        { "G", "3276B1" },
        { "H", "4CAF50" },
        { "I", "FFC107" },
        { "J", "FF9800" },
        { "K", "F44336" },
        { "L", "3276B1" },
        { "M", "4CAF50" },
        { "N", "FFC107" },
        { "O", "FF9800" },
        { "P", "F44336" },
        { "Q", "3276B1" },
        { "R", "4CAF50" },
        { "S", "FFC107" },
        { "T", "FF9800" },
        { "U", "F44336" },
        { "V", "3276B1" },
        { "W", "4CAF50" },
        { "X", "FFC107" },
        { "Y", "FF9800" },
        { "Z", "F44336" },
        { "0", "3276B1" },
        { "1", "4CAF50" },
        { "2", "FFC107" },
        { "3", "FF9800" },
        { "4", "F44336" },
        { "5", "3276B1" },
        { "6", "4CAF50" },
        { "7", "FFC107" },
        { "8", "FF9800" },
        { "9", "FFC107" }


      };

      string key = initial.Substring(0, 1).ToUpper();
      string color = d.ContainsKey(key) ? d[key] : "999999";
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
                SizeF sizeF = gr.MeasureString("XX", font);
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
      return MakeInitialCircle(initial, color, Color.Transparent, size);
    }

    public static Image MakeInitialCircle(string initial, Color color, Color backgroundColor, int size)
    {
      Bitmap bmp = new Bitmap(size, size, PixelFormat.Format32bppPArgb);
      GraphicsPath gp = new GraphicsPath();
      using (Graphics gr = Graphics.FromImage(bmp))
      {
        gr.Clear(backgroundColor);
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
              gr.DrawEllipse(pen, 1, 1, size - 3, size - 3);
              gr.FillEllipse(brush, 1, 1, size - 3, size - 3);
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
      return MakeRound(img, Color.Transparent);
    }

    public static Image MakeRound(Image img, Color backgroundColor)
    {
      Bitmap bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppPArgb);
      GraphicsPath gp = new GraphicsPath();
      using (Graphics gr = Graphics.FromImage(bmp))
      {
        gr.Clear(backgroundColor);
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

    private void ProcessAttachment(HttpContext context, string attachmentID)
    {
      //http://127.0.0.1/tsdev/dc/attachments/7401
      //https://app.teamsupport.com/dc/attachments/{AttachmentID}

      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      // the following is a big hack to get it out fast.... Please do not consider robust code.
      int id;
      if (int.TryParse(attachmentID, out id))
      {
        TeamSupport.Data.Attachment attachment = Attachments.GetAttachment(LoginUser.Anonymous, id);
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
              Ticket ticket = Tickets.GetTicket(action.Collection.LoginUser, action.TicketID);
              if (action.IsVisibleOnPortal)
              {
                if (ticket.IsVisibleOnPortal)
                {
                  Organizations organizations = new Organizations(attachment.Collection.LoginUser);
                  organizations.LoadByTicketID(ticket.TicketID);
                  isAuthenticated = organizations.FindByOrganizationID(user.OrganizationID) != null;
                }
              }

              if (!isAuthenticated)
              {
                isAuthenticated = ticket.IsKnowledgeBase && ticket.IsVisibleOnPortal && action.IsKnowledgeBase && action.IsVisibleOnPortal;              
              }
            }

          }
          catch (Exception)
          {
          }
        }

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

      else 
      {
        SqlCommand command = new SqlCommand();
        command.CommandText = "SELECT AttachmentID FROM Attachments WHERE AttachmentGUID=@AttachmentGUID";
        command.Parameters.AddWithValue("@AttachmentGUID", Guid.Parse(attachmentID));

        id = SqlExecutor.ExecuteInt(LoginUser.Anonymous, command);

        TeamSupport.Data.Attachment attachment = Attachments.GetAttachment(LoginUser.Anonymous,  id);
        Organization organization = Organizations.GetOrganization(attachment.Collection.LoginUser, attachment.OrganizationID);

        if (!organization.AllowUnsecureAttachmentViewing)
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




    }

    private void ProcessImportLog(HttpContext context, int importID)
    {
      System.Web.HttpBrowserCapabilities browser = context.Request.Browser;
      if (browser.Browser != "IE") context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

      //http://127.0.0.1/tsdev/dc/attachments/7401
      //https://app.teamsupport.com/dc/attachments/{AttachmentID}
      Import import = Imports.GetImport(LoginUser.Anonymous, importID);
      Organization organization = Organizations.GetOrganization(import.Collection.LoginUser, import.OrganizationID);
      User user = null;
      bool isAuthenticated = import.OrganizationID == TSAuthentication.OrganizationID;


      if (isAuthenticated)
      {
        user = Users.GetUser(import.Collection.LoginUser, TSAuthentication.UserID);
      }
      else
      {
        context.Response.Write("Unauthorized");
        context.Response.ContentType = "text/html";
        return;
      }

      string logPath = AttachmentPath.GetPath(import.Collection.LoginUser, import.OrganizationID, AttachmentPath.Folder.ImportLogs);
      string fileName = import.ImportID.ToString() + ".txt";
      logPath = Path.Combine(logPath, fileName);

      if (!File.Exists(logPath))
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



      string openType = "attachment";
      string fileType = "text/plain";

      context.Response.AddHeader("Content-Disposition", openType + "; filename=\"" + fileName + "\"");
      //context.Response.AddHeader("Content-Length", attachment.FileSize.ToString());
      context.Response.ContentType = fileType;
      context.Response.WriteFile(logPath);
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
