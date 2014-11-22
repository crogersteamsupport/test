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


namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class WebHooks : ServiceThread
  {

    public override void Run()
    {
      int lastStatusHistoryID = Settings.ReadInt("LastStatusHistoryID", 0);
      lastStatusHistoryID = 8407614;

      if (lastStatusHistoryID < 1)
      { 
        lastStatusHistoryID = (int) SqlExecutor.ExecuteScalar(LoginUser, "SELECT MAX(StatusHistoryID) FROM StatusHistory");
      }

      try
      {
        TicketsView tickets = new TicketsView(LoginUser);
        tickets.LoadNewCustomerResponded(LoginUser, lastStatusHistoryID);
        if (!tickets.IsEmpty) Settings.ReadInt("LastStatusHistoryID", lastStatusHistoryID);
        foreach (TicketsViewItem ticket in tickets)
        {
          SendToSlack(ticket);
        }
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending ticket to slack");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Webhooks", "Error customer responded");
      }
 
    }

    private void SendToSlack(TicketsViewItem ticket)
    {
      //payload={"text": "This is a line of text in a channel.\nAnd this is another line of text."}
      //payload={"text": "A very important thing has occurred! <https://alert-system.com/alerts/1234|Click here> for details!"}
      //https://hooks.slack.com/services/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
      //https://teamsupport.slack.com/services/3065457408?added=1
      //payload={"channel": "#customer-responded", "username": "webhookbot", "text": "This is posted to #customer-responded and comes from a bot named webhookbot.", "icon_emoji": ":ghost:"}
      try
      {
      
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://hooks.slack.com/services/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");
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
          string message = string.Format("{0} responded to Ticket {1}, severity is {2}", ticket.ModifierName, ticket.TicketNumber.ToString(), ticket.Severity);
          SlackPayload payload = new SlackPayload(message, "#customer-responded", "customer-responded-bot");
          streamWriter.Write("payload=" + Uri.EscapeUriString(JsonConvert.SerializeObject(payload)));
          streamWriter.Flush();
          streamWriter.Close();


          var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
          using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
          {
            var result = streamReader.ReadToEnd();
          }
        }
      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending customer responded");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Webhooks", ticket.Row);
      }
    }

  }

  class SlackPayload
  {
    public SlackPayload(string text, string channel, string username)
    {
      this.text = text;
      this.username = username;
      this.channel = channel;
    }
    
    public string text { get; set; }
    public string username { get; set; }
    public string channel { get; set; }
  
  }
}
