using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
  
  public class RestWaterCoolerView
  {
    public static string GetWaterCoolerViewItem(RestCommand command, int )
    {
      WaterCoolerViewItem waterCoolerViewItem = WaterCoolerView.GetWaterCoolerViewItem(command.LoginUser, );
      if (waterCoolerViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return waterCoolerViewItem.GetXml("WaterCoolerViewItem", true);
    }
    
    public static string GetWaterCoolerView(RestCommand command)
    {
      WaterCoolerView waterCoolerView = new WaterCoolerView(command.LoginUser);
      waterCoolerView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return waterCoolerView.GetXml("WaterCoolerView", "WaterCoolerViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
