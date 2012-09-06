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
  [Serializable]
  public class EmailSender : ServiceThread
  {
    private static int[] _nextAttempts = new int[] { 10, 15, 20, 30, 60, 120, 360, 720, 1440 };

    private bool _isDebug = false;
    private string _debugAddresses;

    public EmailSender()
    {
    }

    public override void Run()
    {
      _isDebug = Settings.ReadBool("Debug", false);
      if (_isDebug)
      {
        _debugAddresses = Settings.ReadString("Debug Email Address").Replace(';', ',');
      }

      try
      {
        SendEmails();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error sending emails");
      }

      try
      {
        DeleteOldEmails();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error deleting old emails");
      }

    }

    private void DeleteOldEmails()
    { 
      SqlExecutor.ExecuteNonQuery(LoginUser, new SqlCommand("DELETE FROM Emails WHERE DateCreated < (DATEADD(month, -1, GetUtcDate()))"));
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
            if (_isDebug == true)
            {
              message.To.Clear();
              message.To.Add(_debugAddresses);
              message.Subject = "[DEBUG] " + message.Subject;
            }
            client.Send(message);
            email.IsSuccess = true;
            email.IsWaiting = false;
            email.Body = "";
            email.DateSent = DateTime.UtcNow;
            email.Collection.Save();
            UpdateHealth();
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
    /*
    private static void SmtpSendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
      if (e.Cancelled) return;

      int emailID = (int)e.UserState;
      Email email = Emails.GetEmail(GetLoginUser(), emailID);

      if (e.Error != null)
      {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("AsyncSendError: ");
        builder.AppendLine(e.Error.Message);
        builder.AppendLine("<br />");
        builder.AppendLine("<br />");
        builder.AppendLine("<br />");
        builder.AppendLine(e.Error.StackTrace);
        email.NextAttempt = DateTime.UtcNow.AddMinutes(_nextAttempts[email.Attempts - 1] * email.Attempts);
        email.LastFailedReason = builder.ToString();
        email.IsSuccess = false;
        email.IsWaiting = (email.Attempts < _nextAttempts.Length);
      }
      else
      {
        email.IsSuccess = true;
        email.IsWaiting = false;
        email.Body = "";
        email.DateSent = DateTime.UtcNow;
      }

      email.Collection.Save();
    }
    */
  }
}
