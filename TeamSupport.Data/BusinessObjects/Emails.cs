using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.IO;

namespace TeamSupport.Data
{
  public partial class Email
  {
    public MailMessage GetMailMessage()
    {
      MailMessage message = new MailMessage();
      message.From = GetEmailAddressFromString(FromAddress);
      AddEmailAddressesFromString(message.To, ToAddress);
      AddEmailAddressesFromString(message.CC, CCAddress);
      AddEmailAddressesFromString(message.Bcc, BCCAddress);
      message.Subject = Subject;
      message.Body = Body;
      message.IsBodyHtml = IsHtml;

      if (!string.IsNullOrEmpty(Attachments))
      {
        foreach (string fileName in Attachments.Split(';'))
        {
          try
          {
            message.Attachments.Add(new System.Net.Mail.Attachment(fileName));
          }
          catch (Exception)
          {

          }
        }
      }

      return message;
    }

    private void AddEmailAddressesFromString(MailAddressCollection collection, string text)
    {
      if (string.IsNullOrEmpty(text.Trim())) return;
      string[] list = text.Split('|');
      foreach (string s in list)
      {
        MailAddress address = GetEmailAddressFromString(s);
        if (address != null) collection.Add(address);
      }

    }

    private MailAddress GetEmailAddressFromString(string text)
    {
      string name = "";
      int start = text.IndexOf('"');
      int end = -1;
      if (start > -1)
      {
        start++;
        end = text.IndexOf('"', start + 1);
        if (end > -1)
        {
          name = text.Substring(start, end - start).Trim();
        }
      }

      if (name == "") return new MailAddress(text);
      string address = text;

      start = text.IndexOf('<');
      end = -1;
      if (start > -1)
      {
        start++;
        end = text.IndexOf('>', start + 1);
        if (end > -1)
        {
          address = text.Substring(start, end - start).Trim();
        }
      }

      return new MailAddress(address, name);
    }

  }
  
  public partial class Emails
  {
    public void LoadWaiting()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT * FROM Emails WHERE IsWaiting = 1 AND NextAttempt < GETUTCDATE() ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public void LoadTop100Waiting()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"SELECT TOP 100 * FROM Emails WHERE IsWaiting = 1 AND NextAttempt < GETUTCDATE() ORDER BY DateCreated DESC";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public static string EmailAddressToString(MailAddressCollection addresses)
    {
      StringBuilder builder = new StringBuilder();

      foreach (MailAddress address in addresses)
      {
        if (builder.Length > 0) builder.Append("|");
        if (!string.IsNullOrEmpty(address.DisplayName))
        {
          builder.Append(string.Format("\"{0}\" <{1}>", address.DisplayName, address.Address));
        }
        else
	      {
          builder.Append(address.Address);
	      }
      }

      return builder.ToString();
    }

    public void AddEmail(int organizationID, string description, MailMessage message, string[] attachmentFileNames)
    {
      Email email = AddNewEmail();

      email.OrganizationID = organizationID;
      email.Description = description;
      email.FromAddress = message.From.ToString();
      email.ToAddress = EmailAddressToString(message.To);
      email.CCAddress = EmailAddressToString(message.CC);
      email.BCCAddress = EmailAddressToString(message.Bcc);
      email.Subject = message.Subject;
      email.Body = message.Body;
      email.IsHtml = message.IsBodyHtml;
      email.DateSent = null;
      email.LastFailedReason = "";
      email.IsSuccess = false;
      email.NextAttempt = DateTime.UtcNow;
      email.IsWaiting = true;
      email.Attempts = 0;

      List<string> attachments = new List<string>();
      int size = 0;
      if (attachmentFileNames != null)
      {
        foreach (string fileName in attachmentFileNames)
        {
          if (File.Exists(fileName))
          {
            attachments.Add(fileName);
            size = size + (int)((new FileInfo(fileName)).Length / 1024);
          }
        }
      }
      if (attachments.Count > 0) email.Attachments = string.Join(";", attachments.ToArray());
      email.Size = size;
    }

    public void AddEmail(int organizationID, string description, MailMessage message)
    {
      AddEmail(organizationID, description, message, null);
    }

    public static void AddEmail(LoginUser loginUser, int organizationID, string description, MailMessage message, string[] attachmentFileNames)
    {
      Emails emails = new Emails(loginUser);
      emails.AddEmail(organizationID, description, message, attachmentFileNames);
      emails.Save();
    }

    public static void AddEmail(LoginUser loginUser, int organizationID, string description, MailMessage message)
    {
      Emails.AddEmail(loginUser, organizationID, description, message, null);
    }

  }
  
}
