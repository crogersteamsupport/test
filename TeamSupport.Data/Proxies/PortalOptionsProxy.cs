using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class PortalOption : BaseItem
  {
    public PortalOptionProxy GetProxy()
    {
      PortalOptionProxy result = new PortalOptionProxy();
      result.PortalForward = this.PortalForward;
      result.AllowNameEditing = this.AllowNameEditing;
      result.AllowSeverityEditing = this.AllowSeverityEditing;
      result.DisplayLogout = this.DisplayLogout;
      result.EnableSaExpiration = this.EnableSaExpiration;
      result.DisablePublicMyTickets = this.DisablePublicMyTickets;
      result.RequestAccess = this.RequestAccess;
      result.AutoRegister = this.AutoRegister;
      result.RequestGroup = this.RequestGroup;
      result.RequestType = this.RequestType;
      result.TermsAndConditions = this.TermsAndConditions;
      result.DisplayTandC = this.DisplayTandC;
      result.DisplayAdvArticles = this.DisplayAdvArticles;
      result.TwoColumnFields = this.TwoColumnFields;
      result.PublicLandingPageBody = this.PublicLandingPageBody;
      result.PublicLandingPageHeader = this.PublicLandingPageHeader;
      result.EnableScreenr = this.EnableScreenr;
      result.EnableVideoRecording = this.EnableVideoRecording;
      result.DisplayLandingPage = this.DisplayLandingPage;
      result.LandingPageHtml = this.LandingPageHtml;
      result.DisplayProductVersion = this.DisplayProductVersion;
      result.RestrictProductVersion = this.RestrictProductVersion;
      result.DisplayAdvKB = this.DisplayAdvKB;
      result.DisplayAdvProducts = this.DisplayAdvProducts;
      result.DisplaySettings = this.DisplaySettings;
      result.DisplayPortalPhone = this.DisplayPortalPhone;
      result.DisplayFooter = this.DisplayFooter;
      result.DisplayForum = this.DisplayForum;
      result.DeflectionEnabled = this.DeflectionEnabled;
      result.BasicPortalDirections = this.BasicPortalDirections;
      result.AdvPortalWidth = this.AdvPortalWidth;
      result.Theme = this.Theme;
      result.HideCloseButton = this.HideCloseButton;
      result.HonorSupportExpiration = this.HonorSupportExpiration;
      result.DisplayProducts = this.DisplayProducts;
      result.KBAccess = this.KBAccess;
      result.PortalName = this.PortalName;
      result.DisplayGroups = this.DisplayGroups;
      result.BasicPortalColumnWidth = this.BasicPortalColumnWidth;
      result.HideGroupAssignedTo = this.HideGroupAssignedTo;
      result.HideUserAssignedTo = this.HideUserAssignedTo;
      result.CompanyRequiredInBasic = this.CompanyRequiredInBasic;
      result.UseCompanyInBasic = this.UseCompanyInBasic;
      result.PageBackgroundColor = this.PageBackgroundColor;
      result.FontColor = this.FontColor;
      result.FontFamily = this.FontFamily;
      result.UseRecaptcha = this.UseRecaptcha;
      result.PortalHTMLFooter = this.PortalHTMLFooter;
      result.PortalHTMLHeader = this.PortalHTMLHeader;
      result.OrganizationID = this.OrganizationID;
    
      FacebookOptions fb = new FacebookOptions(BaseCollection.LoginUser);
      fb.LoadByOrganizationID(result.OrganizationID);
      if (fb.IsEmpty)
      {
          result.DisplayFbArticles = false;
          result.DisplayFbKB = false;
      }
      else
      {
          result.DisplayFbArticles = fb[0].DisplayArticles;
          result.DisplayFbKB = fb[0].DisplayKB;
      }       
       
      return result;
    }	
  }
}
