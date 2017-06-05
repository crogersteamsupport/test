using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using System.Text;
using TeamSupport.Data.WebHooks;
using System.Web.Services;
using TeamSupport.WebUtils;

public partial class ServiceStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string name = "All services are ";
        StringBuilder rowBuilder = new StringBuilder();

        bool isFailed = false;
        Services services = new Services(LoginUser.Anonymous);
        services.LoadAll();
        isFailed = false;
        foreach (Service service in services)
        {
            bool isGood = true;
            if (service.LastStartTime == null || !service.Enabled) continue;
            if (DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes > service.HealthMaxMinutes)
            {
                isFailed = true;
                isGood = false;
            }
            AddStatusRow(rowBuilder, service.Row["Name"].ToString(), isGood, (DateTime)service.Row["HealthTime"], service.HealthMaxMinutes);
        }

        litStatusRows.Text = rowBuilder.ToString();


        rowBuilder.Clear();

        isFailed = CheckCount(isFailed, rowBuilder, 10, "Outbound Email Delayed", "There are outgoing email delays of more than 10 minutes, check EmailSender service and Socket Labs", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
AND e.Attempts < 1
AND DATEDIFF(MINUTE, e.DateCreated, GETUTCDATE()) > 10
");

        isFailed = CheckCount(isFailed, rowBuilder, 500, "Outbound Email", "There are too many out going emails pending, check EmailSender service", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
");

        isFailed = CheckCount(isFailed, rowBuilder, 500, "Email Processing", "There are too many records in the EmailPosts table, check EmailProcessor service.", @"
SELECT COUNT(*) FROM EmailPosts
WHERE CreatorID <> -5 AND DATEDIFF(SECOND, GETUTCDATE(), DATEADD(SECOND, HoldTime, DateCreated)) < 0 
");

        isFailed = CheckCount(isFailed, rowBuilder, 500, "Index Processing", "There are too many records waiting to be indexed, check Indexer service on POD-IDX01 service.", @"
SELECT COUNT(*) FROM Organizations o 
WHERE o.IsIndexLocked = 0
AND o.ParentID = 1
AND (IsRebuildingIndex = 0 OR DATEDIFF(SECOND, DateLastIndexed, GETUTCDATE()) > 300)
AND o.IsActive = 1
AND (
  EXISTS (SELECT * FROM Tickets t WHERE t.OrganizationID = o.OrganizationID AND t.NeedsIndexing=1)
 
)
");


        //result.Append(string.Format("<h2 style=\"color:{1};\">{0} is{2} running.</h2>", name, !isFailed ? "green" : "red", !isFailed ? "" : " NOT"));

        litQueryRows.Text = rowBuilder.ToString();

        
        litStatus.Text = !isFailed ? "<div class=\"alert alert-success\">All services are running</div>" : "<div class=\"alert alert-danger\">All services are NOT running</div>";

    }

    private static void AddStatusRow(StringBuilder builder, string name, bool status, DateTime lastTime, int maxTime)
    {
        string progress = @"
<div class=""progress"">
  <div class=""progress-bar progress-bar-{1}"" role=""progressbar"" aria-valuenow=""{0}"" aria-valuemin=""0"" aria-valuemax=""100"" style=""width: {0}%"">
    <span class=""sr-only"">{0}% Complete(success)</span>
  </div>
</div>";
        
        int percent = (int)((1-(DateTime.Now.Subtract(lastTime).TotalMinutes / maxTime)) * 100);
        string progressColor = "success";
        if (percent < 50) progressColor = "warning";
        if (percent < 0)
        {
            progressColor = "danger";
            percent = 100;
        }
        string bar = string.Format(progress, percent.ToString(), progressColor);
        builder.Append(string.Format("<tr class=\"{0}\"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", status ? "" : "danger", name, status ? "Good" : "Down", lastTime, bar));

        if (!status)
        {
            SendMessageToSlack(name + " is down.");

        }
    }

    private static void AddQueryRow(StringBuilder builder, string name, bool status, int max, int count, string message)
    {
        builder.Append(string.Format("<tr class=\"{0}\"><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>", status ? "" : "danger", name, status ? "Good" : "Down", max, count, message));
    }

    private static bool CheckCount(bool isFailed, StringBuilder builder, int maxCount, string name, string failedMessage, string query)
    {
        int result = (int)SqlExecutor.ExecuteScalar(LoginUser.Anonymous, query);
        bool isBad = false;
        if (result > maxCount)
        {
            isBad = true;
            AddQueryRow(builder, name, false, maxCount, result, failedMessage);
            string message = string.Format("Service: {0} ({1}) records - {2}", name, result.ToString(), failedMessage);
            SendMessageToSlack(message);
        }
        else
        {
            string message = failedMessage;
            AddQueryRow(builder, name, true, maxCount, result, "");

        }



        return isFailed || isBad;
    }

    [WebMethod]
    public static void AckSlack()
    {
        
        User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
        SendMessageToSlack(user.FirstLastName + " has acknowledged this issue on pod " + SystemSettings.GetPodName());
    }
    private static void SendMessageToSlack(string messageText)
    {
        messageText = string.Format("{0} - {1}/ServiceStatus.aspx", messageText, SystemSettings.GetAppUrl());
        try
        {
            SlackMessage message = new SlackMessage(LoginUser.Anonymous);
            message.TextPlain = messageText;
            message.TextRich = messageText;
            message.Color = "#D00000";

            // send to channel
            message.Send("#service-warnings");
        }
        catch (Exception ex)
        {

        }
    }

}
