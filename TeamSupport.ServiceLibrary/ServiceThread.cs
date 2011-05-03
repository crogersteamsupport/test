using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;


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

    private bool _isStopped = true;
    public bool IsStopped
    {
      get { lock (this) { return _isStopped; } }
    }

    private string _enableKey;
    private string _intervalKey;
    private int _defaultInterval;

    public void Stop()
    {
      if (IsStopped) return;
      lock (this) { _isStopped = true; }
      _thread.Join(30000);
    }

    public void Start(string enableKey, string intervalKey, int defaultInterval)
    {
      if (!IsStopped) return;
      _enableKey = enableKey;
      _intervalKey = intervalKey;
      _defaultInterval = defaultInterval;

      lock (this) { _isStopped = false; }
      _thread = new Thread(new ThreadStart(Process));
      _thread.Priority = ThreadPriority.Lowest;
      _thread.Start();
    }

    private void Process()
    {
      DateTime lastTime = DateTime.Now;

      while (true)
      {
        try
        {
          if (IsStopped) return;
          if (Utils.GetSettingInt(_enableKey, 0) > 0 && lastTime.AddSeconds(Utils.GetSettingInt(_intervalKey, _defaultInterval)) < DateTime.Now)
          {
            Run();
            lastTime = DateTime.Now;
          }
          if (IsStopped) return;
          Thread.Sleep(1000);
        }
        catch (Exception)
        {

        }
      }
    }

    public abstract void Run();
  }

}
