using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  public partial class TicketSlaViewItem
  {
  }
  
  public partial class TicketSlaView
  {

    public void LoadAllUnnotifiedAndExpired()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText =
@"
SELECT tsv.*
FROM TicketSlaView tsv
LEFT JOIN SlaNotifications sn ON tsv.TicketID = sn.TicketID

WHERE 
(ISNULL(tsv.ViolationTimeClosed, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.ViolationTimeClosed, GETUTCDATE()), ISNULL(sn.TimeClosedViolationDate, 999999)) > 10)
OR
(ISNULL(tsv.WarningTimeClosed, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.WarningTimeClosed, GETUTCDATE()), ISNULL(sn.TimeClosedWarningDate, 999999)) > 10)
OR 
(ISNULL(tsv.ViolationLastAction, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.ViolationLastAction, GETUTCDATE()), ISNULL(sn.LastActionViolationDate, 999999)) > 10)
OR
(ISNULL(tsv.WarningLastAction, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.WarningLastAction, GETUTCDATE()), ISNULL(sn.LastActionWarningDate, 999999)) > 10)
OR 
(ISNULL(tsv.ViolationInitialResponse, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.ViolationInitialResponse, GETUTCDATE()), ISNULL(sn.InitialResponseViolationDate, 999999)) > 10)
OR
(ISNULL(tsv.WarningInitialResponse, 10000) BETWEEN -1440 AND -1
 AND
 DATEDIFF(minute, DATEADD(minute, tsv.WarningInitialResponse, GETUTCDATE()), ISNULL(sn.InitialResponseWarningDate, 999999)) > 10)
";


        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }
  }
  
}
