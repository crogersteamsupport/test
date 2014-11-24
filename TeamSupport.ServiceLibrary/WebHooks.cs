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

    public override void Run()
    {
      int lastStatusHistoryID = Settings.ReadInt("LastStatusHistoryID", 0);
      if (lastStatusHistoryID < 1)
      {
        lastStatusHistoryID = (int)SqlExecutor.ExecuteScalar(LoginUser, "SELECT MAX(StatusHistoryID) FROM StatusHistory");
        Settings.WriteInt("LastStatusHistoryID", lastStatusHistoryID);
      }

      try
      {
        TicketsView tickets = new TicketsView(LoginUser);
        Settings.WriteInt("LastStatusHistoryID", (int)SqlExecutor.ExecuteScalar(LoginUser, "SELECT MAX(StatusHistoryID) FROM StatusHistory"));

        tickets.LoadNewCustomerResponded(LoginUser, lastStatusHistoryID);
        foreach (TicketsViewItem ticket in tickets)
        {
          SendCustomerRespondedToSlack(ticket);
        }

        tickets = new TicketsView(LoginUser);
        tickets.LoadNewUrgentTickets(LoginUser, lastStatusHistoryID);
        foreach (TicketsViewItem ticket in tickets)
        {
          SendUrgentTicketToSlack(ticket);
        }

      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending ticket to slack");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Webhooks", "Error customer responded");
      }
 
    }

    private string GetUserSlackID(int? userID)
    {

      return null;
    }

    private void SendUrgentTicketToSlack(TicketsViewItem ticket)
    { 
    
    
    }

    private void SendCustomerRespondedToSlack(TicketsViewItem ticket)
    {
      try
      {
        Logs.WriteEvent("Processing Customer Responded, Ticket: " + ticket.TicketNumber);

        string user = string.IsNullOrWhiteSpace(ticket.UserName) ? "" : ticket.UserName;
        string customers = string.IsNullOrWhiteSpace(ticket.Customers) ? "" : ticket.Customers;

        SlackMessage message = new SlackMessage();

        message.Channel = "#customer-responded";
        message.TextPlain = string.Format("A customer responded to Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}", ticket.TicketNumber.ToString(), ticket.Name, ticket.Severity, customers, user);
        message.TextRich = string.Format("A customer responded to Ticket {0} - {1}", ticket.TicketNumber.ToString(), ticket.Name);
        message.Color = "#D00000";
        message.Fields.Add(new SlackField("Customers", customers, true));
        message.Fields.Add(new SlackField("Assigned To", user, true));
        message.Fields.Add(new SlackField("Severity", ticket.Severity, true));

        // send to channel
        SendSlackMessage(message);

        // send to user
        string slackUser = GetUserSlackID(ticket.UserID);
        if (!string.IsNullOrWhiteSpace(slackUser))
        {
          message.Channel = "@" + slackUser;
          SendSlackMessage(message);
        }
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending customer responded");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Webhooks", ticket.Row);
      }
    }

    private void SendSlackMessage(SlackMessage message)
    {
      var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://hooks.slack.com/services/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");
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";
      httpWebRequest.Method = "POST";

      using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        dynamic payload = new ExpandoObject();
        payload.channel = message.Channel;
        payload.username = "teamsupport-bot";

        if (message.IsPlain)
        {
          payload.text = WebUtility.HtmlEncode(message.TextPlain);
        }
        else
        {
          List<dynamic> attachments = new List<dynamic>();
          dynamic attachment = new ExpandoObject();
          attachment.fallback = WebUtility.HtmlEncode(message.TextPlain);
          attachment.pretext = WebUtility.HtmlEncode(message.TextRich);
          attachment.color = message.Color;

          List<dynamic> fields = new List<dynamic>();

          foreach (SlackField slackField in message.Fields)
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

  class SlackMessage
  { 
    public SlackMessage()
    {
      Fields = new List<SlackField>();
      IsPlain = false;
    }

    public bool IsPlain { get; set; }
    public string Channel { get; set; }
    public string TextPlain { get; set; }
    public string TextRich { get; set; }
    public string Color { get; set; }
    public List<SlackField> Fields { get; set; }
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
