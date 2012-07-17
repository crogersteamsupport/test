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
  
  public class RestWatercoolerMsg
  {
    public static string GetWatercoolerMsgItem(RestCommand command, int messageID)
    {
      WatercoolerMsgItem watercoolerMsgItem = WatercoolerMsg.GetWatercoolerMsgItem(command.LoginUser, messageID);
      if (watercoolerMsgItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return watercoolerMsgItem.GetXml("WatercoolerMsgItem", true);
    }
    
    public static string GetWatercoolerMsg(RestCommand command)
    {
      WatercoolerMsg watercoolerMsg = new WatercoolerMsg(command.LoginUser);
      watercoolerMsg.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return watercoolerMsg.GetXml("WatercoolerMsg", "WatercoolerMsgItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
