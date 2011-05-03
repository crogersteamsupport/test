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
using Microsoft.Win32;

namespace TeamSupport.Service
{
  public partial class Service : ServiceBase
  {
    EmailProcessor _emailProcessor;
    EmailSender _emailSender;
    SlaProcessor _slaProcessor;
    IisDbLogger _iisDbLogger;
    MurocUpdater _murocUpdater;

    public Service()
    {
      InitializeComponent();
    }

    protected override void OnStart(string[] args)
    {
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
      _emailProcessor = new EmailProcessor();
      _emailSender = new EmailSender();
      _slaProcessor = new SlaProcessor();
      _iisDbLogger = new IisDbLogger();
      _murocUpdater = new MurocUpdater();

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
      _emailProcessor.Stop();
      _emailSender.Stop();
      _slaProcessor.Stop();
      _iisDbLogger.Stop();
      _murocUpdater.Stop();
    }

    private void StartProcessing()
    {
      _emailProcessor.Start("EmailEnabled", "EmailInterval", 10);
      _emailSender.Start("EmailEnabled", "EmailInterval", 10);
      _slaProcessor.Start("SlaProcessEnabled", "SlaProcessInterval", 300);
      _iisDbLogger.Start("IisLogEnabled", "IisLogInterval", 300);
      _murocUpdater.Start("MurocUpdateEnabled", "MurocUpdateInterval", 300);
    }


  }
}
