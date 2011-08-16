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
      int delay = Request["Delay"] != null ? int.Parse(Request["Delay"]) : 20;
      string name = "All services are ";
      
      bool flag = false;
      if (Request["ServiceName"] != null)
      {
        Service service = Services.GetService(LoginUser.Anonymous, Request["ServiceName"], false);
        if (service == null)
        {
          Response.Write("Service not found");
          Response.End();
          return;
        }
        name = service.Name + " service is ";
        if (service.LastStartTime != null) 
        {
          if (DateTime.Now.Subtract((DateTime)service.Row["LastStartTime"]).TotalMinutes < delay)
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
          if (service.LastStartTime == null) continue;
          if (DateTime.Now.Subtract((DateTime)service.Row["LastStartTime"]).TotalMinutes > delay)
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