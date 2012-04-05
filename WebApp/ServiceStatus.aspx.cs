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
        flag = true;
        foreach (Service service in services)
        {
          if (service.LastStartTime == null || !service.Enabled) continue;
          if (DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes > service.HealthMaxMinutes)
          {
            flag = false;
            break;
          }
        }
      }

      if (flag)
      {
        litStatus.Text = "<h2 style=\"color:green;\">"+name+"running.</h2>";
      }
      else
      {
        litStatus.Text = "<h2 style=\"color:red;\">"+name+"not running.</h2>";
      }

    }
}