using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class OrganizationEmail
  {
  }
  
  public partial class OrganizationEmails
  {
    public void LoadByOrganization(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM OrganizationEmails WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadByTemplate(int organizationID, int emailTemplateID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM OrganizationEmails WHERE EmailTemplateID = @EmailTemplateID AND OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@EmailTemplateID", emailTemplateID);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }


  }
  
}
