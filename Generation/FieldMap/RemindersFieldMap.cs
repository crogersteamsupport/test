using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Reminders
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ReminderID", "ReminderID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("Note", "Note", false, false, false);
      _fieldMap.AddMap("DueDate", "DueDate", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("IsComplete", "IsComplete", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
            
    }
  }
  
}
