using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class OrganizationProducts
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationProductID", "OrganizationProductID", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("ProductID", "ProductID", true, true, true);
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", true, true, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", true, true, true);
      _fieldMap.AddMap("ImportID", "ImportID", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
            
    }
  }
  
}
