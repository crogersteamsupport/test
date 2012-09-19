using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using TeamSupport.Data;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace TeamSupport.ServiceLibrary
{
  public class ServiceManager
  {

    List<ServiceThread> _threads;
    LoginUser _loginUser;
    Logs _logs;
    string _domain;

    public ServiceManager()
    {
      _threads = new List<ServiceThread>();
      _logs = new Logs("Service Manager");
      _loginUser = ServiceThread.GetLoginUser("ServiceManager");
      _domain = SystemSettings.ReadString(_loginUser, "AppDomain", "Unknown Domain");
    }

    private Thread _thread;
    public Thread Thread
    {
      get { lock (this) { return _thread; } }
    }

    private bool _isStopped = true;
    public bool IsStopped
    {
      get { lock (this) { return _isStopped; } }
    }

    public void Stop()
    {
      lock (this) { _isStopped = true; }
      _logs.WriteEvent("Stop Requested");
      if (Thread == null) return;
      if (Thread.IsAlive)
      {
        if (!Thread.Join(20000))
        {
          if (Thread.IsAlive)
          {
            _logs.WriteEvent("Still alive, aborting");
            Thread.Abort();
          }
        }
        else
        {
          _logs.WriteEvent("Successfully Stopped");
        }
      }    
    
    }

    public void Start()
    {
      if (!IsStopped) return;
      _logs.WriteHeader("Service Manager Started");
      _logs.WriteEvent("Data Connection Established: " + _loginUser.ConnectionString);
      CreateThreadDomains();
      _isStopped = false;
      _thread = new Thread(new ThreadStart(RunLoop));
      _thread.Start();
    }

    private void RunLoop()
    {
      _isStopped = false;

      foreach (ServiceThread thread in _threads)
      {
        thread.Start();
      }

      while (_isStopped == false)
      {
        CheckHealth();
        Thread.Sleep(1000);
      }

      ServiceThread.ServiceStopped = true;

      foreach (ServiceThread thread in _threads)
      {
        thread.Stop();
      }
    }


    private void CreateThreadDomains()
    {
      Services services = new Data.Services(_loginUser);
      services.LoadAll();

      foreach (TeamSupport.Data.Service service in services)
      {
        if (service.AutoStart == true)
        {
          CreateThreadDomain(service);
        }
      }
    }

    private ServiceThread CreateThreadDomain(TeamSupport.Data.Service service)
    {
      _logs.WriteEvent("Creating app domain for " + service.Name);
      try
      {
        string servicePath = AppDomain.CurrentDomain.BaseDirectory;
        AppDomainSetup setup = new AppDomainSetup();
        setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        Type type = Type.GetType(string.Format("{0}.{1}, {2}", service.NameSpace, service.Name, service.AssemblyName), true);
        AppDomain domain = AppDomain.CreateDomain(type.FullName, AppDomain.CurrentDomain.Evidence, setup);
        ServiceThread thread = (ServiceThread)domain.CreateInstanceFromAndUnwrap(Path.Combine(servicePath, service.AssemblyName + ".dll"), type.FullName);
        _threads.Add(thread);
        return thread;
      }
      catch (Exception ex)
      {
        _logs.WriteEvent("Error starting appdomain for " + service.Name);
        _logs.WriteException(ex);
        return null;

      }
    }

    private void EmailMessage(string message)
    {
      MailMessage mailMessage = new MailMessage();
      mailMessage.From = new MailAddress("kjones@teamsupport.com", "TeamSupport Service Manager");
      mailMessage.To.Add("kjones@teamsupport.com");
      mailMessage.To.Add("jharada@teamsupport.com");
      mailMessage.Subject = "Service Manager Notice";
      mailMessage.Body = string.Format("{0} <br/><br/> {1}", message, _domain);

      SmtpClient client = new SmtpClient();
      Settings settings = new Settings(_loginUser, "EmailSender");

      client = new SmtpClient(settings.ReadString("SMTP Host"), settings.ReadInt("SMTP Port"));
      string username = settings.ReadString("SMTP UserName", "");
      if (username.Trim() != "") client.Credentials = new System.Net.NetworkCredential(settings.ReadString("SMTP UserName"), settings.ReadString("SMTP Password"));
      client.Send(mailMessage);
      _logs.WriteEvent("Message Sent: " + message);
    }

    private void CheckHealth()
    {
      try
      {
        // make sure all are running
        Services services = new Data.Services(_loginUser);
        services.LoadAll();


        foreach (TeamSupport.Data.Service service in services)
        {
          if (service.HealthTime == null || !service.Enabled || !service.AutoStart) continue;

          if (FindServiceThread(service.Name) == null)
          {
            _logs.WriteEvent("Unable to find thread: " + service.Name);

            //RecreateThread(service);
          }
        }

        // make sure all are healthy
        foreach (ServiceThread thread in _threads)
        {
          try 
	        {	        
            TeamSupport.Data.Service service = Services.GetService(_loginUser, thread.ServiceName);
            if (service.HealthTime == null || !service.Enabled || !service.AutoStart) continue;

            if (DateTime.Now.Subtract((DateTime)service.Row["HealthTime"]).TotalMinutes > service.HealthMaxMinutes)
            {
              RecreateThread(service, thread);
              return;
            }
	        }
	        catch (Exception ex)
	        {
            ExceptionLogs.LogException(_loginUser, ex, "Service Manager: Error checking thread health");
            try 
	          {	        
		          _threads.Remove(thread);
              return;
	          }
	          catch (Exception)
	          {
	          }
	        }
        }


      }
      catch (Exception ex)
      {
        _logs.WriteException(ex);
        ExceptionLogs.LogException(_loginUser, ex, "Service Manager: Health Timer");
      }
    }

    private ServiceThread FindServiceThread(string serviceName)
    {
      foreach (ServiceThread thread in _threads)
      {
        try
        {
          if (thread.ServiceName.ToLower() == serviceName.ToLower())
          {
            return thread;
          }

        }
        catch (Exception ex)
        {
          try
          {
            _logs.WriteEvent("Exception finding thread: " + serviceName);
            _logs.WriteException(ex);
            _logs.WriteEvent("Thread Count: " + _threads.Count.ToString());
          }
          catch (Exception)
          {
            
          }
        }
      }
      _logs.WriteEvent("Thread not found: " + serviceName);
      return null;
    }

    private void RecreateThread(Service service, ServiceThread thread)
    {
      _logs.WriteEvent(string.Format("Attempting to restart {0}.  Current Time: {1:g}   Last Health Check: {2:g}",
        service.Name,
        DateTime.Now,
        (DateTime)service.Row["HealthTime"]));

      if (thread != null)
      {
        try
        {
          thread.Stop();
        }
        catch (Exception ex)
        {
          _logs.WriteException(ex);
          ExceptionLogs.LogException(_loginUser, ex, "Service Manager: Thread Stop");
        }

        try
        {
          _threads.Remove(thread);
        }
        catch (Exception)
        {
        }
      }

      ServiceThread newThread = CreateThreadDomain(service);
      if (newThread != null)
      {
        newThread.Start();
        _logs.WriteEvent("Restarted " + newThread.ServiceName);
        EmailMessage("Restarted " + newThread.ServiceName);
      }
      else
      {
        _logs.WriteEvent("FAILED to restart " + service.Name);
        EmailMessage("FAILED to restart " + service.Name);
      }

    }

    private void RecreateThread(Service service)
    {
      RecreateThread(service, null);
    }


  }
}
