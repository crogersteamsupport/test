using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dtSearch.Engine;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;

namespace TeamSupport.ServiceLibrary
{
  public class TransLocks : ServiceThread
  {
    public override void Run()
    {
      
      try
      {
        using(SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
	      {
          connection.Open();
          SqlCommand command = new SqlCommand(
@"
INSERT INTO TransLocks 
SELECT  L.request_session_id AS SPID, 
        DB_NAME(L.resource_database_id) AS DatabaseName,
        O.Name AS LockedObjectName, 
        P.object_id AS LockedObjectId, 
        L.resource_type AS LockedResource, 
        L.request_mode AS LockType,
        ST.text AS SqlStatementText,        
        ES.login_name AS LoginName,
        ES.program_name AS AppName,
        ES.host_name AS HostName,
        TST.is_user_transaction as IsUserTransaction,
        AT.name as TransactionName,
        CN.auth_scheme as AuthenticationMethod,
        GETDATE() AS DateCreated
FROM    sys.dm_tran_locks L
        JOIN sys.partitions P ON P.hobt_id = L.resource_associated_entity_id
        JOIN sys.objects O ON O.object_id = P.object_id
        JOIN sys.dm_exec_sessions ES ON ES.session_id = L.request_session_id
        JOIN sys.dm_tran_session_transactions TST ON ES.session_id = TST.session_id
        JOIN sys.dm_tran_active_transactions AT ON TST.transaction_id = AT.transaction_id
        JOIN sys.dm_exec_connections CN ON CN.session_id = ES.session_id
        CROSS APPLY sys.dm_exec_sql_text(CN.most_recent_sql_handle) AS ST
WHERE   resource_database_id = 5
ORDER BY L.request_session_id DESC
", connection);

          command.ExecuteNonQuery();
          connection.Close();
	      }



      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "TransLocks"); 
      }
    }

    public override string ServiceName
    {
      get { return "TransLocks"; }
    }
  }
}
