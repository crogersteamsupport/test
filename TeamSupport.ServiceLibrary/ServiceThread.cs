using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using TeamSupport.Data;
using System.Configuration;

namespace TeamSupport.ServiceLibrary
{
  public abstract class ServiceThread
  {
    public ServiceThread()
    {
      _loginUser = GetLoginUser(ServiceName);
      _settings = new Settings(_loginUser, ServiceName);
    }

    private static object _staticLock = new object();

    private static bool _serviceStopped = false;
    public static bool ServiceStopped
    {
      get { lock (_staticLock) { return ServiceThread._serviceStopped; } }
      set { lock (_staticLock) { ServiceThread._serviceStopped = value; } }
    }

    private Thread _thread;
    public Thread Thread
    {
      get { lock (this) { return _thread; } }
    }

    private bool _isLoop = true;
    public bool IsLoop
    {
      get { lock (this) { return _isLoop; } }
      set { lock (this) { _isLoop = value; } }
    }

    protected bool _isStopped = true;
    public virtual  bool IsStopped
    {
      get 
      { 
        lock (this) { return _isStopped || ServiceStopped; } 
      }
    }

    private bool _runHandlesStop = false;
    protected bool RunHandlesStop
    {
      get { lock (this) { return _runHandlesStop; } }
      set { lock (this) { _runHandlesStop = value; } }
    }

    public abstract string ServiceName { get; }

    protected Settings _settings;
    protected Settings Settings
    {
      get { return _settings; }
    }

    protected LoginUser _loginUser;
    protected LoginUser LoginUser
    {
      get { return _loginUser; }
    }

    public virtual void Stop()
    {
      lock (this) { _isStopped = true; }

      if (Thread.IsAlive)
      {
        if (!Thread.Join(10000))
        {
          if (Thread.IsAlive) _thread.Abort();
        }
      }
    }

    public virtual void Start()
    {
      if (!IsStopped) return;
      _isStopped = false;
      _thread = new Thread(new ThreadStart(Process));
      _thread.Priority = ThreadPriority.Lowest;

      if (IsLoop)
      {
        Service service = Services.GetService(_loginUser, ServiceName);
        service.RunCount = 0;
        service.RunTimeAvg = 0;
        service.RunTimeMax = 0;
        service.ErrorCount = 0;
        service.LastError = "";
        service.LastResult = "";
        service.Collection.Save();
      }

      _thread.Start();
    }

    protected virtual void Process()
    {
      try
      {
        DateTime lastTime = DateTime.Now;

        while (true)
        {
          Service service = Services.GetService(_loginUser, ServiceName);
          try
          {
            if (IsStopped && !_runHandlesStop) return;
            if (service.Enabled && (lastTime.AddSeconds(service.Interval) < DateTime.Now || !IsLoop))
            {
              service.LastStartTime = DateTime.Now;
              service.Collection.Save();
              Run();
              lastTime = DateTime.Now;
              service.RunCount = service.RunCount + 1;
              service.LastEndTime = DateTime.Now;
              service.LastResult = "Success";
              int total = (int)((DateTime)service.LastEndTime).Subtract((DateTime)service.LastStartTime).TotalSeconds;
              service.RunTimeMax = service.RunTimeMax < total ? total : service.RunTimeMax;
              
              service.RunTimeAvg = service.RunCount > 1 ? (int)((((service.RunCount - 1) * service.RunTimeAvg) + total) / service.RunCount) : total;
              service.Collection.Save();
            }
            Thread.Sleep(1000);
          }
          catch (ThreadAbortException)
          { 
          
          }
          catch (Exception ex)
          {
            ExceptionLog log = ExceptionLogs.LogException(_loginUser, ex, "Service - " + ServiceName);
            service.LastError = string.Format("[{0} {1}", log.ExceptionLogID.ToString(), ex.Message);
            service.ErrorCount = service.ErrorCount + 1;
            service.Collection.Save();
          }
          if (!IsLoop)
          {
            Stop();
            return;
          }
        }
      }
      catch (Exception)
      {

      }
    }
    
    ///<summary>This function is called by the service to run a specified interval.  This function is called in its own thread.</summary>
    ///<remarks>Check the property IsStopped to see if you need to stop processing.</remarks>
    public abstract void Run();


    public static LoginUser GetLoginUser(string appName)
    {
      string constring = "Application Name=TeamSupport Service: " + appName + "; " + ConfigurationManager.AppSettings["ConnectionString"];
      return new LoginUser(constring, -1, -1, null);
    }
    /*
    protected static string GetSettingString(string keyName) { return GetSettingString(keyName, ""); }
    protected static string GetSettingString(string keyName, string defaultValue) { return (string)GetSetting(keyName, defaultValue, RegistryValueKind.String); }
    protected static int GetSettingInt(string keyName, int defaultValue) { return (int)GetSetting(keyName, defaultValue, RegistryValueKind.DWord); }
    protected static int GetSettingInt(string keyName) { return (int)GetSetting(keyName, -1, RegistryValueKind.DWord); }

    protected static void SetSettingValue(string keyName, object value, RegistryValueKind kind)
    {
      RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\TeamSupport\Service");
      key.SetValue(keyName, value, kind);
    }

    protected static object GetSetting(string keyName, object defaultValue, RegistryValueKind kind)
    {
      RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\TeamSupport\Service");
      object o = key.GetValue(keyName);
      if (o != null)
      {
        return o;
      }
      else
      {
        key.SetValue(keyName, defaultValue, kind);
        return defaultValue;
      }
    }
*/

  }

}
