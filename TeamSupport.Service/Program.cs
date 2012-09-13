using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TeamSupport.Data;
using TeamSupport.ServiceLibrary;
using TeamSupport.CrmIntegration;
using Microsoft.Win32;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;

namespace TeamSupport.Service
{
  class Program : ServiceBase
  {
    Logs _logs;
    ServiceManager _serviceManager;
 
    static void Main(string[] args)
    {
      ServiceBase.Run(new Program());
    }

    public Program()  
    {
      this.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
    }

    private void TestConnection()
    {
      LoginUser loginUser = ServiceThread.GetLoginUser("ServiceManager");
      string connectionString = loginUser.ConnectionString;
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        try
        {
          connection.Open();
          connection.Close();
        }
        catch (Exception ex)
        {
          throw new Exception(connectionString + " " + ex.Message, ex);
        }
      }
    }


    protected override void OnStart(string[] args)
    {
      _logs = new Logs("Service Base");
      _logs.WriteEvent("OnStart");
      TestConnection();
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
      _serviceManager = new ServiceManager();
      _serviceManager.Start();
    }

    protected override void OnStop()
    {
      _logs.WriteEvent("OnStop");
      _serviceManager.Stop();
    }

    protected override void OnShutdown()
    {
      _logs.WriteEvent("OnShutdown");
      _serviceManager.Stop();
    }

    protected override void OnContinue()
    {
      _logs.WriteEvent("OnContinue");
      base.OnContinue();
    }

    protected override void OnPause()
    {
      _logs.WriteEvent("OnPause");
      base.OnPause();
    }

    protected override void OnCustomCommand(int command)
    {
      _logs.WriteEvent("OnCustomCommand: " + command.ToString());
      base.OnCustomCommand(command);
    }

    protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
    {
      _logs.WriteEvent("OnPowerEvent: " + powerStatus.ToString());

      return base.OnPowerEvent(powerStatus);
    }

    protected override void OnSessionChange(SessionChangeDescription changeDescription)
    {
      _logs.WriteEvent("OnSessionChange: " + changeDescription);
      base.OnSessionChange(changeDescription);
    }
  }
}
