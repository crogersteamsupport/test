using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using System.Text;

public partial class ServiceStatus : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string name = "All services are ";
        StringBuilder result = new StringBuilder();

        bool isFailed = false;
        if (Request["ServiceName"] != null)
        {
            Service service = Services.GetService(LoginUser.Anonymous, Request["ServiceName"], false);
            if (service == null)
            {
                Response.Write("Service not found.");
                Response.End();
                return;
            }

            if (!service.Enabled)
            {
                Response.Write("Service is disabled.");
                Response.End();
                return;
            }

            name = service.Name + " service is ";
            if (service.LastStartTime != null)
            {
                if (DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes > service.HealthMaxMinutes)
                {
                    isFailed = true;
                }
            }
        }
        else
        {
            Services services = new Services(LoginUser.Anonymous);
            services.LoadAll();
            result.Append("<table>");
            isFailed = false;
            foreach (Service service in services)
            {
                string color = "green";
                if (service.LastStartTime == null || !service.Enabled) continue;
                if (DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes > service.HealthMaxMinutes)
                {
                    isFailed = true;
                    color = "red";
                }
                AddRow(result, service.Row["Name"].ToString(), string.Format("Health Time: {0},  Max Minutes: {1}", (DateTime)service.Row["HealthTime"], service.HealthMaxMinutes),color);
            }

            result.Append("</table>");
        }


        isFailed = CheckCount(isFailed, result, 0, "Outbound Email Delay", "There are outgoing email delays of more than 10 minutes, check EmailSender service", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
AND e.Attempts < 1
AND DATEDIFF(MINUTE, e.DateCreated, GETUTCDATE()) > 10
");

        isFailed = CheckCount(isFailed, result, 500, "Outbound Email Back Up", "There are too many out going emails pending, check EmailSender service", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
");

        isFailed = CheckCount(isFailed, result, 1000, "Email Processing Backed Up", "There are too many records in the EmailPosts table, check EmailProcessor service.", @"
SELECT COUNT(*) FROM EmailPosts 
");

        result.Append(string.Format("<h2 style=\"color:{1};\">{0} is{2} running.</h2>", name, !isFailed ? "green" : "red", !isFailed ? "" : " NOT"));
        litStatus.Text = result.ToString();

    }

    private static void AddRow(StringBuilder builder, string name, string message, string color)
    {
        builder.Append(string.Format("<tr style=\"color:{2}\"><td>{0}: </td><td>{1}</td></tr>", name, message, color));
    }

    private static bool CheckCount(bool isFailed, StringBuilder builder, int maxCount, string name, string failedMessage, string query)
    {
        int result = (int)SqlExecutor.ExecuteScalar(LoginUser.Anonymous, query);
        string color = "green";
        bool isBad = false;
        if (result > maxCount)
        {
            isBad = true;
            color = "red";
        }

        AddRow(builder, name, failedMessage, color);


        return isFailed || isBad;
    }
}
