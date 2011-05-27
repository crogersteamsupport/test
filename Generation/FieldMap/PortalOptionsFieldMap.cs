using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class PortalOptions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("PortalHTMLHeader", "PortalHTMLHeader", false, false, false);
      _fieldMap.AddMap("PortalHTMLFooter", "PortalHTMLFooter", false, false, false);
      _fieldMap.AddMap("UseRecaptcha", "UseRecaptcha", false, false, false);
      _fieldMap.AddMap("FontFamily", "FontFamily", false, false, false);
      _fieldMap.AddMap("FontColor", "FontColor", false, false, false);
      _fieldMap.AddMap("PageBackgroundColor", "PageBackgroundColor", false, false, false);
      _fieldMap.AddMap("UseCompanyInBasic", "UseCompanyInBasic", false, false, false);
      _fieldMap.AddMap("CompanyRequiredInBasic", "CompanyRequiredInBasic", false, false, false);
      _fieldMap.AddMap("HideUserAssignedTo", "HideUserAssignedTo", false, false, false);
      _fieldMap.AddMap("HideGroupAssignedTo", "HideGroupAssignedTo", false, false, false);
      _fieldMap.AddMap("BasicPortalColumnWidth", "BasicPortalColumnWidth", false, false, false);
      _fieldMap.AddMap("DisplayGroups", "DisplayGroups", false, false, false);
      _fieldMap.AddMap("PortalName", "PortalName", false, false, false);
      _fieldMap.AddMap("KBAccess", "KBAccess", false, false, false);
      _fieldMap.AddMap("DisplayProducts", "DisplayProducts", false, false, false);
      _fieldMap.AddMap("HonorSupportExpiration", "HonorSupportExpiration", false, false, false);
      _fieldMap.AddMap("HideCloseButton", "HideCloseButton", false, false, false);
      _fieldMap.AddMap("Theme", "Theme", false, false, false);
      _fieldMap.AddMap("AdvPortalWidth", "AdvPortalWidth", false, false, false);
      _fieldMap.AddMap("BasicPortalDirections", "BasicPortalDirections", false, false, false);
      _fieldMap.AddMap("DeflectionEnabled", "DeflectionEnabled", false, false, false);
            
    }
  }
  
}
