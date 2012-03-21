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
  
  public class RestReportEmails
  {
    public static string GetReportEmail(RestCommand command, int reportEmailID)
    {
      ReportEmail reportEmail = ReportEmails.GetReportEmail(command.LoginUser, reportEmailID);
      if (reportEmail.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportEmail.GetXml("ReportEmail", true);
    }
    
    public static string GetReportEmails(RestCommand command)
    {
      ReportEmails reportEmails = new ReportEmails(command.LoginUser);
      reportEmails.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportEmails.GetXml("ReportEmails", "ReportEmail", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
