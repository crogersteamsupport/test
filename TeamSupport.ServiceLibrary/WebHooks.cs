using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;
using TeamSupport.Data.WebHooks;



namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class WebHooks : ServiceThread
  {

    public override void Run()
    {
      try
      {

      }
      catch (Exception ex)
      {
        Logs.WriteEvent("Error sending to slack");
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "Webhooks", "Error to slack");
      }

    }
  }
}
