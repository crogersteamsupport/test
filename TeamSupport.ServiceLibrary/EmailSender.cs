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
  public class EmailSender : ServiceThreadPoolProcess
  {
    public EmailSender(int threadPosition) : base(threadPosition) { }

    private static int[] _nextAttempts = new int[] { 10, 15, 20, 30, 60, 120, 360, 720, 1440 };

    private bool _isDebug = false;
    private string _debugAddresses;

    public override void Run()
    {
      _isDebug = Settings.ReadBool("Debug", false);
      if (_isDebug)
      {
        _debugAddresses = Settings.ReadString("Debug Email Address").Replace(';', ',');
      }

      try
      {
        SendNextEmail();
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending email");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error sending email");
      }

      try
      {
        DeleteOldEmails();
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error deleting old emails");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error deleting old emails");
      }

    }

    private void DeleteOldEmails()
    {
      SqlExecutor.ExecuteNonQuery(LoginUser, new SqlCommand("DELETE FROM Emails WHERE DateCreated < (DATEADD(month, -1, GetUtcDate()))"));
    }


    private void SendNextEmail()
    {
      string processID = Guid.NewGuid().ToString();
      Email email = Emails.GetNextWaiting(LoginUser, processID);
      if (email == null) return;
      
      Logs.WriteLine();
      Logs.WriteHeader("Processing Email");
      Logs.WriteEventFormat("EmailID: {0}, EmailPostID: {1}", email.EmailID.ToString(), email.EmailPostID.ToString());

      try
      {
        SmtpClient client = new SmtpClient();
        client = new SmtpClient(Settings.ReadString("SMTP Host"), Settings.ReadInt("SMTP Port"));
        string username = Settings.ReadString("SMTP UserName", "");
        if (username.Trim() != "") client.Credentials = new System.Net.NetworkCredential(Settings.ReadString("SMTP UserName"), Settings.ReadString("SMTP Password"));
        Logs.WriteEventFormat("SMTP: Host: {0}, Port: {1}, User: {2}", client.Host, client.Port.ToString(), username);
        email.Attempts = email.Attempts + 1;
        email.Collection.Save();
        Logs.WriteEvent("Attempt: " + email.Attempts.ToString());
        Logs.WriteEventFormat("Size: {0}, Attachments: {1}", email.Size.ToString(), email.Attachments);
        MailMessage message = email.GetMailMessage();
        message.Headers.Add("X-xsMessageId", email.OrganizationID.ToString());
        message.Headers.Add("X-xsMailingId", email.EmailID.ToString());
        if (_isDebug == true)
        {
          message.To.Clear();
          message.To.Add(_debugAddresses);
          message.Subject = "[DEBUG] " + message.Subject;
        }
        Logs.WriteEvent("Sending email");
        client.Send(message);
        Logs.WriteEvent("Email sent");
        email.IsSuccess = true;
        email.IsWaiting = false;
        email.Body = "";
        email.DateSent = DateTime.UtcNow;
        email.LockProcessID = null;
        email.Collection.Save();
        UpdateHealth();
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending email");
        Logs.WriteException(ex);

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
        email.LockProcessID = null;
        email.Collection.Save();
      }
    }

  }
}
