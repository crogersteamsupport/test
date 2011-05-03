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
            
    }
  }
  
}
