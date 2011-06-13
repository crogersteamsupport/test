using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  public abstract class ServiceThread
  {
    public ServiceThread()
    {
    }

    private Thread _thread;
    public Thread Thread
    {
      get { return _thread; }
      set { _thread = value; }
    }

    private bool _isLoop = true;
    public bool IsLoop
    {
      get { lock (this) { return _isLoop; } }
      set { lock (this) { _isLoop = value; } }
    }

    private bool _isComplete = false;
    public bool IsComplete
    {
      get
      {
        lock (this)
        {
          return _isComplete;
        } 
      }
    }  

    private bool _isStopped = true;
    public bool IsStopped
    {
      get 
      { 
        lock (this) { return _isStopped; } 
      }
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

    /// <summary>
    /// Stops the thread
    /// </summary>
    public virtual void Stop()
    {
      if (IsStopped) return;
      lock (this) { _isStopped = true; }
      _thread.Join(30000);
    }



    /// <summary>
    /// Starts the thread
    /// </summary>
    public virtual void Start()
    {
      if (!IsStopped) return;
      _loginUser = GetLoginUser(ServiceName);
      _settings = new Settings(_loginUser, ServiceName);
      lock (this) { _isStopped = false; }
      _thread = new Thread(new ThreadStart(Process));
      _thread.Priority = ThreadPriority.Lowest;
      _thread.Start();
    }

    protected virtual void Process()
    {
      DateTime lastTime = DateTime.Now;

      while (true)
      {
        Service service = Services.GetService(_loginUser, ServiceName);
        try
        {
          if (IsStopped) return;
          if (service.Enabled && (lastTime.AddSeconds(service.Interval) < DateTime.Now || !IsLoop))
          {
            service.LastStartTime = DateTime.Now;
            service.Collection.Save();
            Run();
            lastTime = DateTime.Now;
            service.RunCount = service.RunCount + 1;
            service.LastEndTime = DateTime.Now;
            service.LastResult = "Success";
            int total = (int)service.LastStartTime.Subtract(service.LastEndTime).TotalSeconds;
            service.RunTimeMax = service.RunTimeMax < total ? total : service.RunTimeMax;
            service.RunTimeAvg = (int)((((service.RunCount -1) * service.RunTimeAvg) + total) / service.RunCount);
            service.Collection.Save();
          }
          if (IsStopped) return;
          Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
          ExceptionLog log = ExceptionLogs.LogException(_loginUser, ex, "Service - " + ServiceName);
          service.LastError = string.Format("[{0}} {1}", log.ExceptionLogID.ToString(), ex.Message);
          service.ErrorCount = service.ErrorCount + 1;
          service.Collection.Save();
        }
        if (!IsLoop)
        {
          lock (this) { _isComplete = true; }
          return;
        }
      }
    }
    
    ///<summary>This function is called by the service to run a specified interval.  This function is called in its own thread.</summary>
    ///<remarks>Check the property IsStopped to see if you need to stop processing.</remarks>
    public abstract void Run();


    protected static LoginUser GetLoginUser(string appName)
    {
      string defaultConString = "Application Name=TeamSupport Service: APPNAME;Data Source=localhost;Initial Catalog=TeamSupport;Persist Security Info=True;User ID=sa;Password=muroc";
      string constring = GetSettingString("ConnectionString", defaultConString).Replace("APPNAME", appName);
      return new LoginUser(constring, -1, -1, null);
    }

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


  }

}
