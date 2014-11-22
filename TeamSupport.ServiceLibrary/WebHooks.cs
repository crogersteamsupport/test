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
        Logs.WriteEvent("Processing Ticket " + ticket.TicketNumber);

        ActionsView actions = new ActionsView(ticket.Collection.LoginUser);
        actions.LoadLatestByTicket(ticket.TicketID, true);

        string body = actions.IsEmpty ? "" : actions[0].Description;
        string customer = actions.IsEmpty ? "" : (string.IsNullOrEmpty(actions[0].ModifierName) ? "" : actions[0].ModifierName);
        string user = string.IsNullOrWhiteSpace(ticket.UserName) ? "" : ticket.UserName;
        string customers = string.IsNullOrWhiteSpace(ticket.Customers) ? "" : ticket.Customers;
      
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://hooks.slack.com/services/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");
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
          string message = string.Format("{0} responded to Ticket {1}, severity is {2}", ticket.ModifierName, ticket.TicketNumber.ToString(), ticket.Severity);

          string payload = @"
{
  ""channel"": ""#customer-responded"", 
  ""username"": ""customer-responded-bot"", 
  ""text"": ""A customer responded to Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}"", 
   ""attachments"":[
      {
         ""fallback"":""A customer responded to Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}"",
         ""pretext"":""A customer responded to Ticket {0} - {1}\nSeverity is {2}\nCustomers are {3}\nAssigned to {4}"",
         ""color"":""#D00000"",
         ""fields"":[
            {
               ""title"":""{5}"",
               ""value"":""{6}"",
               ""short"":false
            }
         ]
      }
   ]

}
";

          streamWriter.Write("payload=" + Uri.EscapeUriString(string.Format(payload, ticket.TicketNumber.ToString(), ticket.Name, ticket.Severity, customers, user, customer, body)));
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


}
