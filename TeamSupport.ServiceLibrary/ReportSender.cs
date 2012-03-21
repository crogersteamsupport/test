using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;


namespace TeamSupport.ServiceLibrary
{
  public class ReportSender : ServiceThread
  {
    private static int[] _nextAttempts = new int[] { 10, 15, 20, 30, 60, 120, 360, 720, 1440 };

    private bool _isDebug = false;
    private MailAddressCollection _debugAddresses;

    public ReportSender()
    {
      _debugAddresses = new MailAddressCollection();
    }

    public override string ServiceName
    {
      get { return "ReportSender"; }
    }

    public override void Run()
    { /*
      _isDebug = Settings.ReadBool("Debug", false);
      _debugAddresses.Clear();

      try
      {
        string[] addresses = Settings.ReadString("Debug Email Address").Split(';');
        foreach (string item in addresses) { _debugAddresses.Add(new MailAddress(item.Trim())); }
      }
      catch (Exception)
      {
      }


        EmailPosts emailPosts = new EmailPosts(LoginUser);
        emailPosts.LoadReportEmails();

        foreach (EmailPost emailPost in emailPosts)
        {
          if (emailPost.EmailPostType != EmailPostType.SendReport) continue;

          if (DateTime.UtcNow > ((DateTime)emailPost.Row["DateCreated"]).AddSeconds(emailPost.HoldTime) || _isDebug)
          {
            try
            {
              SetTimeZone(emailPost);
              ProcessEmail(emailPost);
            }
            catch (Exception ex)
            {
              ExceptionLogs.LogException(LoginUser, ex, "Email", emailPost.Row);
            }
            emailPost.Collection.DeleteFromDB(emailPost.EmailPostID);
          }
          System.Threading.Thread.Sleep(0);
          if (IsStopped) break;
        }
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error processing emails");
      }

      try
      {
        SendEmails();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error sending emails");
      }*/
    }

    /*
    public void ProceesReportEmail(int reportID, int userID)
    {
      User user = Users.GetUser(LoginUser, userID);
      Report report = Reports.GetReport(LoginUser, reportID);

      if (report == null || (report.OrganizationID != LoginUser.OrganizationID && report.OrganizationID != null)) { return; }

      string sql = report.GetSql(false);
      SqlCommand command = new SqlCommand(sql);
      Report.CreateParameters(LoginUser, command, userID);
      string text = DataUtils.CommandToCsv(LoginUser, command, false);
      /*
      MemoryStream stream = new MemoryStream();
      ZipOutputStream zip = new ZipOutputStream(stream);
      zip.SetLevel(9);
      zip.PutNextEntry(new ZipEntry(report.Name + ".csv"));
      Byte[] bytes = Encoding.UTF8.GetBytes(text);
      zip.Write(bytes, 0, bytes.Length);
      zip.CloseEntry();
      zip.Finish();
      stream.WriteTo(context.Response.OutputStream);
      //context.Response.ContentType = "application/x-zip-compressed";
      context.Response.ContentType = "application/octet-stream";
      context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + report.Name + ".zip\"");
      context.Response.AddHeader("Content-Length", stream.Length.ToString());
      zip.Close();* /

      context.Response.Write(text);
      context.Response.ContentType = "application/octet-stream";
      context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + report.Name + ".csv\"");
      //context.Response.AddHeader("Content-Length", text.Length.ToString());
      
      
      MailMessage message = new MailMessage();
      message.To.Add(new MailAddress(user.Email, user.FirstLastName));
      message.From = new MailAddress("support@teamsupport.com");
      message.IsBodyHtml = true;
      message.Body = "Test";
      message.Subject = "Test";


      MemoryStream stream = new MemoryStream();
      ZipOutputStream zip = new ZipOutputStream(stream);
      zip.SetLevel(9);
      zip.PutNextEntry(new ZipEntry(report.Name + ".csv"));
      Byte[] bytes = Encoding.UTF8.GetBytes(text);
      zip.Write(bytes, 0, bytes.Length);
      zip.CloseEntry();
      zip.Finish();
      message.Attachments.Add(new System.Net.Mail.Attachment(stream, report.Name));

      
      zip.Close();
      

      
    }

    private void SendEmails()
    {
      Emails emails = new Emails(LoginUser);
      emails.LoadTop100Waiting();
      if (emails.IsEmpty) return;

      SmtpClient client = new SmtpClient();
      client = new SmtpClient(Settings.ReadString("SMTP Host"), Settings.ReadInt("SMTP Port"));
      string username = Settings.ReadString("SMTP UserName", "");
      if (username.Trim() != "") client.Credentials = new System.Net.NetworkCredential(Settings.ReadString("SMTP UserName"), Settings.ReadString("SMTP Password"));

      foreach (Email email in emails)
      {
        if (IsStopped) break;
        try
        {
          if (email.NextAttempt < DateTime.UtcNow)
          {

            email.Attempts = email.Attempts + 1;
            email.Collection.Save();
            MailMessage message = email.GetMailMessage();
            message.Headers.Add("X-xsMessageId", email.OrganizationID.ToString());
            message.Headers.Add("X-xsMailingId", email.EmailID.ToString());
            client.Send(message);
            email.IsSuccess = true;
            email.IsWaiting = false;
            email.Body = "";
            email.DateSent = DateTime.UtcNow;
            email.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          StringBuilder builder = new StringBuilder();
          builder.AppendLine(ex.Message);
          builder.AppendLine("<br />");
          builder.AppendLine("<br />");
          builder.AppendLine("<br />");
          builder.AppendLine(ex.StackTrace);
          email.NextAttempt = DateTime.UtcNow.AddMinutes(_nextAttempts[email.Attempts - 1] * email.Attempts);
          email.LastFailedReason = builder.ToString();
          email.IsSuccess = false;
          email.IsWaiting = (email.Attempts < _nextAttempts.Length);
          email.Collection.Save();
        }
      }
    }
*/    
  }
}
