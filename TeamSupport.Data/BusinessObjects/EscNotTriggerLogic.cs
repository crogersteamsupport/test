using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class EscNotTriggerLogicItem
  {
  }
  
  public partial class EscNotTriggerLogic
  {

    public void LoadByTriggerID(int triggerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT t.*, rtf.FieldName FROM EscNotTriggerLogic t LEFT JOIN ReportTableFields rtf ON rtf.ReportTableFieldID = t.FieldID WHERE rtf.ReportTableID = 10 AND t.TriggerID = @TriggerID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@TriggerID", triggerID);
        Fill(command, "EscNotTriggerLogic");
      }
    }
  }
  
}
