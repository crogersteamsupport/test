using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Drawing;

namespace TeamSupport.Data.Quarantine
{
    public class ContentHandlerQ
    {
        public static void ProcessImages(HttpContext context, string[] segments, int organizationID)
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
                if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);
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

            if (File.Exists(fileName))
            {
                WriteImage(context, fileName);
            }

            //Organization organization = Organizations.GetOrganization(LoginUser.Anonymous, organizationID);
            //bool isAuthenticated = organizationID == TSAuthentication.OrganizationID;

            //if (isAuthenticated || organization.AllowUnsecureAttachmentViewing)
            //{
            //	if (Path.GetExtension(path) == "")
            //	{
            //		path = Path.ChangeExtension(path, ".jpg");
            //		string imageFile = Path.GetFileName(path);
            //		path = Path.GetDirectoryName(path);
            //		string imagePath = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.Images), path);
            //		if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);
            //		fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
            //		if (!File.Exists(fileName))
            //		{
            //			imagePath = Path.Combine(AttachmentPath.GetDefaultPath(LoginUser.Anonymous, AttachmentPath.Folder.Images), path);
            //			fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
            //		}

            //	}
            //	else
            //	{
            //		fileName = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.Images), path);
            //	}

            //	if (File.Exists(fileName))
            //	{
            //		WriteImage(context, fileName);
            //	}
            //}
            //else
            //{
            //	context.Response.Write("Unauthorized");
            //	context.Response.ContentType = "text/html";
            //	return;
            //}
        }

        private static void WriteImage(HttpContext context, string fileName)
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

        public static void ProcessRatingImages(HttpContext context, string[] segments, int organizationID)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < segments.Length; i++)
            {
                if (i != 1)
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

        public static void ProcessChat(HttpContext context, string command, int organizationID)
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

        public static void ProcessAvatar(HttpContext context, string[] segments, int organizationID, string path, int filePathID)
        {
            string fileName = "";
            if (Path.GetExtension(path) == "")
            {
                path = Path.ChangeExtension(path, ".jpg");
                string imageFile = Path.GetFileName(path);
                path = Path.GetDirectoryName(path);
                string imagePath = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.ProfileImages, filePathID), path);
                fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
                if (!File.Exists(fileName))
                {
                    imagePath = Path.Combine(AttachmentPath.GetDefaultPath(LoginUser.Anonymous, AttachmentPath.Folder.ProfileImages, filePathID), path);
                    fileName = AttachmentPath.GetImageFileName(imagePath, imageFile);
                }

            }
            else
            {
                fileName = Path.Combine(AttachmentPath.GetPath(LoginUser.Anonymous, organizationID, AttachmentPath.Folder.ProfileImages, filePathID), path);
            }
            if (File.Exists(fileName)) WriteImage(context, fileName);
        }


    }
}
