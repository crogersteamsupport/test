using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TeamSupport.Data;
using TeamSupport.ServiceLibrary;
using TeamSupport.CrmIntegration;
using Microsoft.Win32;

namespace TeamSupport.Service
{
  public partial class Service : ServiceBase
  {
    List<ServiceThread> _threads;
    EmailProcessor _emailProcessor;
    EmailSender _emailSender;
    SlaProcessor _slaProcessor;
    Indexer _indexer;

    public Service()
    {
      InitializeComponent();
      _threads = new List<ServiceThread>();
      _threads.Add(new EmailProcessor());
      _threads.Add(new EmailSender());
      _threads.Add(new SlaProcessor());
      _threads.Add(new Indexer());
      _threads.Add(new CrmPool());
    }

    protected override void OnStart(string[] args)
    {
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
      StartProcessing();
    }

    protected override void OnStop()
    {
      StopProcessing();
    }

    protected override void OnShutdown()
    {
      StopProcessing();
    }

    private void StopProcessing()
    {
      foreach (ServiceThread thread in _threads)
      {
        thread.Stop();
      }
    }

    private void StartProcessing()
    {
      foreach (ServiceThread thread in _threads)
      {
        thread.Start();
      }
    }


  }
}
