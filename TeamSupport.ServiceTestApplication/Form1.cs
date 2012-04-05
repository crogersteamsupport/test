using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;
using TeamSupport.ServiceLibrary;
using TeamSupport.CrmIntegration;
using Microsoft.Win32;
using System.Net.Mail;
using System.IO;

namespace TeamSupport.ServiceTestApplication
{
  public partial class Form1 : Form
  {

    List<ServiceThread> _threads;
    List<AppDomain> _domains;
    ServiceManager _serviceManager;
    
    public Form1()
    {
      InitializeComponent();
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
      Settings settings = new Settings(ServiceThread.GetLoginUser("Service Test App"), "EmailSender");
      settings.WriteBool("Debug", true);
      settings = new Settings(ServiceThread.GetLoginUser("Service Test App"), "EmailProcessor");
      settings.WriteBool("Debug", true);
      _threads = new List<ServiceThread>();
      _domains = new List<AppDomain>();
      _serviceManager = new ServiceManager();
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.Save();
      StopAll();
    }

    private void StopAll()
    {
      ServiceThread.ServiceStopped = true;
      foreach (ServiceThread thread in _threads)
      {
        thread.Stop();
      }
    }

    private void StartProcess(ServiceThread thread, Button button)
    {
      button.Text = button.Text.Replace("Start ", "Stop ");
      button.ForeColor = Color.Red;
      try
      {
        button.Enabled = false;
        thread.Start();
      }
      finally
      {
        button.Enabled = true;
      }
    
    }

    private void StopProcess(ServiceThread thread, Button button)
    {
      try
      {
        button.Enabled = false;
        thread.Stop();
        thread = null;
      }
      finally
      {
        button.Enabled = true;
        button.Text = button.Text.Replace("Stop ", "Start ");
        button.ForeColor = Color.Green;
      }
    }

    private void btnThread_Click(object sender, EventArgs e)
    {
      Button button = sender as Button;
      ServiceThread thread = null;
      switch (button.Name)
      {
        case "btnEmailProcessor": thread = FindServiceThread("EmailProcessor") ?? CreateThreadDomain("TeamSupport EmailProcessor", "TeamSupport.ServiceLibrary.dll", typeof(EmailProcessor)); break;
        case "btnEmailSender": thread = FindServiceThread("EmailSender") ?? CreateThreadDomain("TeamSupport EmailSender", "TeamSupport.ServiceLibrary.dll", typeof(EmailSender)); break;
        case "btnReminders": thread = FindServiceThread("ReminderProcessor") ?? CreateThreadDomain("TeamSupport ReminderProcessor", "TeamSupport.ServiceLibrary.dll", typeof(ReminderProcessor)); break;
        case "btnIndexer": thread = FindServiceThread("Indexer") ?? CreateThreadDomain("TeamSupport Indexer", "TeamSupport.ServiceLibrary.dll", typeof(Indexer)); break;
        case "btnIndexMaint": thread = FindServiceThread("IndexMaintenance") ?? CreateThreadDomain("TeamSupport IndexMaintenance", "TeamSupport.ServiceLibrary.dll", typeof(IndexMaintenance)); break;
        case "btnSlaProcessor": thread = FindServiceThread("SlaProcessor") ?? CreateThreadDomain("TeamSupport SlaProcessor", "TeamSupport.ServiceLibrary.dll", typeof(SlaProcessor)); break;
        case "btnCrmPool": thread = FindServiceThread("CrmPool") ?? CreateThreadDomain("TeamSupport CrmPool", "TeamSupport.CrmIntegration.dll", typeof(CrmPool)); break;
        default: MessageBox.Show("Could not find the service by the button name."); break;
      }

      if (thread == null) return;
      if (thread.IsStopped)
      {
        StartProcess(thread, button);
      }
      else
      {
        StopProcess(thread, button);
      }
    }
    
    private ServiceThread CreateThreadDomain(string domainName, string assemblyName, Type type)
    {
      string servicePath = AppDomain.CurrentDomain.BaseDirectory;
      AppDomainSetup setup = new AppDomainSetup();
      setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
      setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
      AppDomain domain = AppDomain.CreateDomain(domainName, AppDomain.CurrentDomain.Evidence, setup);
      MessageBox.Show(type.AssemblyQualifiedName);
      ServiceThread thread = (ServiceThread)domain.CreateInstanceFromAndUnwrap(Path.Combine(servicePath, assemblyName), type.FullName);
      _threads.Add(thread);
      _domains.Add(domain);
      return thread;
    }


    private ServiceThread FindServiceThread(string serviceName)
    {
      foreach (ServiceThread thread in _threads)
      {
        if (thread.ServiceName == serviceName)
        {
          return thread;
        }
      }

      return null;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (button1.Text.IndexOf("Start") > -1)
      {
        button1.Text = "Stop Service Thread";
        _serviceManager.Start();
      
      }
      else
      {
        button1.Text = "Start Service Thread";
        _serviceManager.Stop();
      }
      

    }

    
  }
}
