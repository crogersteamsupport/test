using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class ImportProcessor : ServiceThread
  {

    public override void Run()
    {
      try
      {
        Imports imports = new Imports(LoginUser);
        imports.LoadWaiting();
        if (!imports.IsEmpty)
        {
          ProcessImport(imports[0]);
        }
        UpdateHealth();
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "ReminderProcessor"); 
      }
    }

    private void ProcessImport(Import import)
    {
      Logs.WriteLine();
      Logs.WriteEvent("***********************************************************************************");
      Logs.WriteEvent("Processing Import  ImportID: " + import.ImportID.ToString());
      Logs.WriteData(import.Row);
      Logs.WriteLine();
      Logs.WriteEvent("***********************************************************************************");
      
    }


  }
}
