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
using Newtonsoft.Json;
using System.Dynamic;
using System.Web;


namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class WebHooks : ServiceThread
  {
    private LoginUser _loginUser;
    public override void Run()
    {
      _loginUser = new Data.LoginUser(LoginUser.ConnectionString, -1, 1078, null);

      int lastStatusHistoryID = Settings.ReadInt("LastStatusHistoryID", 0);
      lastStatusHistoryID = 8426935;
      if (lastStatusHistoryID < 1)
      {
        lastStatusHistoryID = (int)SqlExecutor.ExecuteScalar(_loginUser, "SELECT MAX(StatusHistoryID) FROM StatusHistory");
        Settings.WriteInt("LastStatusHistoryID", lastStatusHistoryID);
      }

      try
      {
        TicketsView tickets = new TicketsView(_loginUser);
        Settings.WriteInt("LastStatusHistoryID", (int)SqlExecutor.ExecuteScalar(_loginUser, "SELECT MAX(StatusHistoryID) FROM StatusHistory"));

        tickets.LoadNewCustomerResponded(_loginUser, lastStatusHistoryID);
        foreach (TicketsViewItem ticket in tickets)
        {
          SendCustomerRespondedToSlack(ticket);
        }


        /*
         * There is no easy way to get the change set of severity yet.  Holding off
        tickets = new TicketsView(_loginUser);
        tickets.LoadNewUrgentTickets(_loginUser, lastStatusHistoryID);
        foreach (TicketsViewItem ticket in tickets)
        {
          SendUrgentTicketToSlack(ticket);
        }
         */

      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending to slack");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(_loginUser, ex, "Webhooks", "Error to slack");
      }
 
    }

    private void SendUrgentTicketToSlack(TicketsViewItem ticket)
    {
      try
      {
        string user = string.IsNullOrWhiteSpace(ticket.UserName) ? "" : ticket.UserName;
        string customers = string.IsNullOrWhiteSpace(ticket.Customers) ? "" : ticket.Customers;
        SlackMessage message = new SlackMessage(_loginUser);
        message.TextPlain = string.Format("Urgent Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}", ticket.TicketNumber.ToString(), ticket.Name, ticket.Severity, customers, user);
        message.TextRich = string.Format("Urgent Ticket {0} - {1}", ticket.TicketNumber.ToString(), ticket.Name);
        message.Color = "#D00000";
        message.Fields.Add(new SlackField("Customers", customers, true));
        message.Fields.Add(new SlackField("Assigned To", user, true));
        message.Fields.Add(new SlackField("Severity", ticket.Severity, true));

        // send to channel
        message.Send("#urgent-tickets");
        // send to user
        message.Send(ticket.UserID);
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending urgent ticket");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(_loginUser, ex, "Webhooks", ticket.Row);
      }
    
    
    }

    private void SendCustomerRespondedToSlack(TicketsViewItem ticket)
    {
      try
      {
        string user = string.IsNullOrWhiteSpace(ticket.UserName) ? "" : ticket.UserName;
        string customers = string.IsNullOrWhiteSpace(ticket.Customers) ? "" : ticket.Customers;
        SlackMessage message = new SlackMessage(_loginUser);
        message.TextPlain = string.Format("A customer responded to Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}", ticket.TicketNumber.ToString(), ticket.Name, ticket.Severity, customers, user);
        message.TextRich = string.Format("A customer responded to Ticket {0} - {1}", ticket.TicketNumber.ToString(), ticket.Name);
        message.Color = "#D00000";
        message.Fields.Add(new SlackField("Customers", customers, true));
        message.Fields.Add(new SlackField("Assigned To", user, true));
        message.Fields.Add(new SlackField("Severity", ticket.Severity, true));

        // send to channel
        message.Send("#customer-responded");
        // send to user
        message.Send(ticket.UserID);
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending customer responded");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(_loginUser, ex, "Webhooks", ticket.Row);
      }
    }
  }

  class SlackMessage
  { 
    public SlackMessage(LoginUser loginUser)
    {
      Fields = new List<SlackField>();
      IsPlain = false;
      LoginUser = loginUser;
    }
    public LoginUser LoginUser { get; set; }
    public bool IsPlain { get; set; }
    public string TextPlain { get; set; }
    public string TextRich { get; set; }
    public string Color { get; set; }
    public List<SlackField> Fields { get; set; }
    private string GetUserSlackID(int userID)
    {
      CustomValue customValue = CustomValues.GetValue(LoginUser, userID, "slackname");
      if (customValue != null && !string.IsNullOrWhiteSpace(customValue.Value)) return customValue.Value;
      return null;
    }
    public void Send(int? userID)
    {
      if (userID == null) return;
      string slackUser = GetUserSlackID((int)userID);

      if (!string.IsNullOrWhiteSpace(slackUser))
      {
        Send("@" + slackUser);
      }
    }
    public void Send(string channel)
    {
      var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://hooks.slack.com/services/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");
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";
      httpWebRequest.Method = "POST";

      using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        dynamic payload = new ExpandoObject();
        payload.channel = channel;
        payload.username = "TeamSupport";

        if (this.IsPlain)
        {
          payload.text = WebUtility.HtmlEncode(this.TextPlain);
        }
        else
        {
          List<dynamic> attachments = new List<dynamic>();
          dynamic attachment = new ExpandoObject();
          attachment.fallback = WebUtility.HtmlEncode(this.TextPlain);
          attachment.pretext = WebUtility.HtmlEncode(this.TextRich);
          attachment.color = this.Color;

          List<dynamic> fields = new List<dynamic>();

          foreach (SlackField slackField in this.Fields)
          {
            dynamic field = new ExpandoObject();
            field.title = WebUtility.HtmlEncode(slackField.Title);
            field.value = WebUtility.HtmlEncode(slackField.Value);
            field.shortxxx = slackField.IsShort;
            fields.Add(field);
          }

          attachment.fields = fields.ToArray();
          attachments.Add(attachment);
          payload.attachments = attachments.ToArray();
        }

        string json = JsonConvert.SerializeObject(payload);

        streamWriter.Write("payload=" + Uri.EscapeUriString(json.Replace("shortxxx", "short")));
        streamWriter.Flush();
        streamWriter.Close();

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
          var result = streamReader.ReadToEnd();
        }
      }
    }
  }

  class SlackField
  {
    public SlackField(string title, string value, bool isShort)
    {
      Title = title;
      Value = value;
      IsShort = isShort;
    }
    
    public string Title { get; set; }
    public string Value { get; set; }
    public bool IsShort { get; set; }
  }


}
