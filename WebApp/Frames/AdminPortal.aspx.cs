using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;

public partial class Frames_AdminPortal : BaseFramePage
{
  protected void Page_Load(object sender, EventArgs e)
  {
   
    btnEdit.OnClientClick = "ShowDialog(top.GetPortalOptionsDialog(" + UserSession.LoginUser.OrganizationID.ToString() + ")); return false;";
    Organization organization = (Organization)Organizations.GetOrganization(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    PortalOption portalOption = (PortalOption)PortalOptions.GetPortalOption(UserSession.LoginUser, UserSession.LoginUser.OrganizationID);
    if (portalOption == null)
    {
      portalOption = (new PortalOptions(UserSession.LoginUser)).AddNewPortalOption();
      portalOption.OrganizationID = UserSession.LoginUser.OrganizationID;
      portalOption.Collection.Save();
    }

    StringBuilder builder = new StringBuilder();
    bool isLicensed = true;

    if (organization.IsAdvancedPortal)
    {
      if (organization.IsBasicPortal)
        lblLicense.Text = "You are licensed for the Advanced and Basic Portals.";
      else
        lblLicense.Text = "You are licensed for the Advanced Portal only.";
    }
    else
    {
      if (organization.IsBasicPortal)
        lblLicense.Text = "You are licensed for the Basic Portal only.";
      else
      {
        lblLicense.Text = "You currently have no portal licenses.  Please contact our sales department at 800.596.2820 x806 or send an email to <a href=\"mailto:sales@teamsupport.com\">sales@teamsupport.com</a>";
        isLicensed = false;
      }
    }


    btnEdit.Visible = isLicensed;
    if (isLicensed)
    {
      /*"Ticket Submission Portal: ticket.TeamSupport.com/sandbox"
"Public Knowledge Base: kb.TeamSupport.com/sandbox"
"Advanced Portal: portal.TeamSupport.com/sandbox"*/

      if (organization.IsBasicPortal)
      {
        builder.Append("<tr><td><strong>Ticket Submission Portal:</strong></td><td>");
        string portalLink = "http://ticket.TeamSupport.com/" + portalOption.PortalName;
        builder.Append(@"<a href=""" + portalLink + @""" target=""PortalLink"" onclick=""window.open('" + portalLink + @"', 'PortalLink')"">" + portalLink + "</a>");
        builder.Append("</td></tr>");
      }

      if (organization.IsBasicPortal)
      {
        builder.Append("<tr><td><strong>Public Knowledge Base:</strong></td><td>");
        string portalLink = "http://kb.TeamSupport.com/" + portalOption.PortalName;
        builder.Append(@"<a href=""" + portalLink + @""" target=""PortalLink"" onclick=""window.open('" + portalLink + @"', 'PortalLink')"">" + portalLink + "</a>");
        builder.Append("</td></tr>");
      }

      if (organization.IsAdvancedPortal)
      {
        builder.Append("<tr><td><strong>Articles Portal:</strong></td><td>");
        string articleLink = "http://articles.TeamSupport.com/" + portalOption.PortalName;
        builder.Append(@"<a href=""" + articleLink + @""" target=""PortalLink"" onclick=""window.open('" + articleLink + @"', 'PortalLink')"">" + articleLink + "</a>");
        builder.Append("</td></tr>");

        builder.Append("<tr><td><strong>Advanced Portal:</strong></td><td>");
        string portalLink = "http://portal.TeamSupport.com/" + portalOption.PortalName;
        builder.Append(@"<a href=""" + portalLink + @""" target=""PortalLink"" onclick=""window.open('" + portalLink + @"', 'PortalLink')"">" + portalLink + "</a>");
        builder.Append("</td></tr>");
      }

      builder.Append("<tr><td><strong>External Portal Link:</strong></td><td>");
      string externalLink = Settings.OrganizationDB.ReadString("ExternalPortalLink", "");
      if (externalLink != "")
        builder.Append(@"<a href=""" + externalLink + @""" target=""PortalLink"" onclick=""window.open('" + externalLink + @"', 'PortalLink')"">" + externalLink + "</a>");
      else
        builder.Append(@"[No external link is assigned]");
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Default Portal Group:</strong></td><td>");
      if (organization.DefaultPortalGroupID != null)
      {
        Group group = Groups.GetGroup(UserSession.LoginUser, (int)organization.DefaultPortalGroupID);
        if (group != null) builder.Append(group.Name);
      }
      else
      {
        builder.Append("[No group is assigned]");
      }
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Use Company in Basic Portal:</strong></td><td>");
      builder.Append(portalOption.UseCompanyInBasic == null ? "False" : ((bool)portalOption.UseCompanyInBasic).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Company Required in Basic Portal:</strong></td><td>");
      builder.Append(portalOption.CompanyRequiredInBasic == null ? "False" : ((bool)portalOption.CompanyRequiredInBasic).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Hide User Assigned To:</strong></td><td>");
      builder.Append(portalOption.HideUserAssignedTo == null ? "False" : ((bool)portalOption.HideUserAssignedTo).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Hide Group Assigned To:</strong></td><td>");
      builder.Append(portalOption.HideGroupAssignedTo == null ? "False" : ((bool)portalOption.HideGroupAssignedTo).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Use Recaptcha:</strong></td><td>");
      builder.Append(portalOption.UseRecaptcha == null ? "True" : ((bool)portalOption.UseRecaptcha).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Allow Access to Public Knowledgebase:</strong></td><td>");
      builder.Append(portalOption.KBAccess == null ? "True" : ((bool)portalOption.KBAccess).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Allow Access to Public Articles:</strong></td><td>");
      builder.Append(organization.IsPublicArticles.ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Display Product Pulldown in Portals:</strong></td><td>");
      builder.Append(portalOption.DisplayProducts == null ? "True" : ((bool)portalOption.DisplayProducts).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Display Product Version Pulldown in Portals:</strong></td><td>");
      builder.Append(portalOption.DisplayProductVersion.ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Display Group Pulldown in Portals:</strong></td><td>");
      builder.Append(portalOption.DisplayGroups == null ? "True" : ((bool)portalOption.DisplayGroups).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Basic Portal Header Message:</strong></td><td>");
      builder.Append(portalOption.BasicPortalDirections == null ? "" : portalOption.BasicPortalDirections);
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Basic Portal Width:</strong></td><td>");
      builder.Append(portalOption.BasicPortalColumnWidth == null ? "N/A" :((int)portalOption.BasicPortalColumnWidth).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Advance Portal Width:</strong></td><td>");
      builder.Append(portalOption.AdvPortalWidth == null ? "N/A" : ((int)portalOption.AdvPortalWidth).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Theme:</strong></td><td>");
      builder.Append(string.IsNullOrEmpty(portalOption.Theme) ? "N/A" : (Enums.PortalThemeNames[portalOption.Theme]).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Use Landing Page:</strong></td><td>");
      builder.Append(portalOption.DisplayLandingPage.ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Ticket Deflection Enabled:</strong></td><td>");
      builder.Append(portalOption.DeflectionEnabled == null ? "False" : ((bool)portalOption.DeflectionEnabled).ToString());
      builder.Append("</td></tr>");

      builder.Append("<tr><td><strong>Enable Screen Recording:</strong></td><td>");
      builder.Append(portalOption.EnableScreenr.ToString());
      builder.Append("</td></tr>");
      /*
            builder.Append("<tr><td><strong>Portal Header HTML/CSS:</strong></td><td>");
            if (portalOption != null)
              builder.Append(portalOption.PortalHTMLHeader);
            else
              builder.Append("[No header is defined]");
            builder.Append("</td></tr>");*/

      /*builder.Append("<tr><td><strong>Portal Footer HTML:</strong></td><td>");
      if (portalOption != null)
        builder.Append(portalOption.PortalHTMLFooter);
      else
        builder.Append("[No footer is defined]");
      builder.Append("</td></tr>");*/

      builder.Append("</table>");
      litDetails.Text = builder.ToString();
    
    }




  }
}
