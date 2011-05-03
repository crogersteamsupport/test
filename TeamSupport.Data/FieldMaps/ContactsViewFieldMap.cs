using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ContactsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Email", "Email", true, true, true);
      _fieldMap.AddMap("FirstName", "FirstName", true, true, true);
      _fieldMap.AddMap("UserID", "ContactID", true, true, true);
      _fieldMap.AddMap("MiddleName", "MiddleName", true, true, true);
      _fieldMap.AddMap("LastName", "LastName", true, true, true);
      _fieldMap.AddMap("Title", "Title", true, true, true);
      _fieldMap.AddMap("IsActive", "IsActive", true, true, true);
      _fieldMap.AddMap("MarkDeleted", "MarkDeleted", false, false, false);
      _fieldMap.AddMap("LastLogin", "LastLogin", false, false, true);
      _fieldMap.AddMap("LastActivity", "LastActivity", false, false, true);
      _fieldMap.AddMap("LastPing", "LastPing", false, false, true);
      _fieldMap.AddMap("IsSystemAdmin", "IsSystemAdmin", false, false, false);
      _fieldMap.AddMap("IsFinanceAdmin", "IsFinanceAdmin", false, false, false);
      _fieldMap.AddMap("IsPasswordExpired", "IsPasswordExpired", false, false, false);
      _fieldMap.AddMap("IsPortalUser", "IsPortalUser", true, true, true);
      _fieldMap.AddMap("PrimaryGroupID", "PrimaryGroupID", true, true, true);
      _fieldMap.AddMap("InOffice", "InOffice", true, true, true);
      _fieldMap.AddMap("InOfficeComment", "InOfficeComment", true, true, true);
      _fieldMap.AddMap("ActivatedOn", "ActivatedOn", true, true, true);
      _fieldMap.AddMap("DeactivatedOn", "DeactivatedOn", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("Organization", "Organization", true, true, true);
      _fieldMap.AddMap("LastVersion", "LastVersion", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("OrganizationParentID", "OrganizationParentID", false, false, false);
      _fieldMap.AddMap("CryptedPassword", "CryptedPassword", true, true, true);
            
    }
  }
  
}
