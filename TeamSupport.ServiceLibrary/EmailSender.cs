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
    private static int[] _nextAttempts = new int[] { 10, 15, 20, 30, 60, 120, 360, 720, 1440 };
    private bool _isDebug = false;
    private static object _staticLock = new object();

    private static Email GetNextEmail(string connectionString, int lockID)
    {
      Email result;
      LoginUser loginUser = new LoginUser(connectionString, -1, -1, null);
      lock (_staticLock) { result = Emails.GetNextWaiting(loginUser, lockID.ToString()); }

      return result;
    }

    public override void ReleaseAllLocks()
    {
      Emails.UnlockAll(LoginUser);
    }

    public override void Run()
    {
      _isDebug = Settings.ReadBool("Debug", false);

      while (!IsStopped)
      {
        try
        {
          Email email = GetNextEmail(LoginUser.ConnectionString, (int)_threadPosition);
          if (email == null) return;
          SendEmail(email);
        }
        catch (Exception ex)
        {
          Logs.WriteEvent("Error sending email");
          Logs.WriteException(ex);
          ExceptionLogs.LogException(LoginUser, ex, "Email", "Error sending email");
        }
      }
 
    }

    private void SendEmail(Email email)
    {
      Logs.WriteLine();
      Logs.WriteHeader("Processing Email");
      Logs.WriteEventFormat("EmailID: {0}, EmailPostID: {1}", email.EmailID.ToString(), email.EmailPostID.ToString());

      try
      {
        using (SmtpClient client = new SmtpClient(Settings.ReadString("SMTP Host"), Settings.ReadInt("SMTP Port")))
        {
          string username = Settings.ReadString("SMTP UserName", "");
          if (username.Trim() != "") client.Credentials = new System.Net.NetworkCredential(Settings.ReadString("SMTP UserName"), Settings.ReadString("SMTP Password"));
          Logs.WriteEventFormat("SMTP: Host: {0}, Port: {1}, User: {2}", client.Host, client.Port.ToString(), username);
          client.Timeout = 500000;
          email.Attempts = email.Attempts + 1;
          email.Collection.Save();
          Logs.WriteEvent("Attempt: " + email.Attempts.ToString());
          Logs.WriteEventFormat("Size: {0}, Attachments: {1}", email.Size.ToString(), email.Attachments);
          MailMessage message = email.GetMailMessage();
          message.Headers.Add("X-xsMessageId", email.OrganizationID.ToString());
          message.Headers.Add("X-xsMailingId", email.EmailID.ToString());
          if (_isDebug == true)
          {
            string debugAddresses = Settings.ReadString("Debug Email Address").Replace(';', ',');
            Logs.WriteEvent("DEBUG Addresses: " + debugAddresses);

            message.To.Clear();
            message.To.Add(debugAddresses);
            message.Subject = "[DEBUG] " + message.Subject;
          }
          Logs.WriteEvent("Sending email");
          client.Send(message);
          Logs.WriteEvent("Email sent");
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
        Logs.WriteEvent("Error sending email");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, _threadPosition.ToString() + " - Email Sender", email.Row);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Log File: " + _threadPosition.ToString());
        builder.AppendLine(ex.Message);
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
