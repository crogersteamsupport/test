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

        bool flag = false;
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
                if (DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes < service.HealthMaxMinutes)
                {
                    flag = true;
                }
            }
        }
        else
        {
            Services services = new Services(LoginUser.Anonymous);
            services.LoadAll();
            result.Append("<table>");
            flag = true;
            foreach (Service service in services)
            {
                if (service.LastStartTime == null || !service.Enabled) continue;
                if (DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes > service.HealthMaxMinutes)
                {
                    flag = false;
                    AddRow(result, service.Row["Name"].ToString(), "Past Health Time");
                }
            }

            result.Append("</table>");
        }


        flag = CheckCount(flag, result, 0, "Outbound Email Delay", "There are outgoing email delays of more than 10 minutes, check EmailSender service", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
AND e.Attempts < 1
AND DATEDIFF(MINUTE, e.DateCreated, GETUTCDATE()) > 10
");

        flag = CheckCount(flag, result, 500, "Outbound Email Back Up", "There are too many out going emails pending, check EmailSender service", @"
SELECT COUNT(*)
FROM Emails e 
WHERE IsWaiting = 1 
");

        flag = CheckCount(flag, result, 1000, "Email Processing Backed Up", "There are too many records in the EmailPosts table, check EmailProcessor service.", @"
SELECT COUNT(*) FROM EmailPosts 
");

        result.Append(string.Format("<h2 style=\"color:{1};\">{0} is{2} running.</h2>", name, flag ? "green" : "red", flag ? "" : " NOT"));
        litStatus.Text = result.ToString();

    }

    private static void AddRow(StringBuilder builder, string name, string message)
    {
        builder.Append(string.Format("<tr><td>{0}: </td><td>{1}</td></tr>", name, message));
    }

    private static bool CheckCount(bool flag, StringBuilder builder, int maxCount, string name, string failedMessage, string query)
    {
        int result = (int)SqlExecutor.ExecuteScalar(LoginUser.Anonymous, query);

        if (result > maxCount)
        {
            AddRow(builder, name, failedMessage);
            return false;
        }

        return flag;
    }
}
