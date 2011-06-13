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
  public class EmailSender : ServiceThread
  {
    private static int[] _nextAttempts = new int[] { 10, 15, 20, 30, 60, 120, 360, 720, 1440 };

    private bool _isDebug = false;
    private MailAddressCollection _debugAddresses;

    public EmailSender()
    {
      _debugAddresses = new MailAddressCollection();
    }

    public override string ServiceName
    {
      get { return "EmailSender"; }
    }

    public override void Run()
    {
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


      try
      {
        SendEmails();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Email", "Error sending emails");
      }
    }

    private void SendEmails()
    {
      Emails emails = new Emails(LoginUser);
      emails.LoadTop100Waiting();
      if (emails.IsEmpty) return;

      SmtpClient client = new SmtpClient();
      client = new SmtpClient(Settings.ReadString("SMTP Host"), Settings.ReadInt("SMTP Port"));
      client.Credentials = new System.Net.NetworkCredential(Settings.ReadString("SMTP UserName"), Settings.ReadString("SMTP Password"));

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
