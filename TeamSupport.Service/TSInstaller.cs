using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace TeamSupport.Service
{
  [RunInstaller(true)]
  public class TSInstaller : Installer
  {
    public TSInstaller()
    {
      var processInstaller = new ServiceProcessInstaller();
      var serviceInstaller = new ServiceInstaller();

      //set the privileges
      processInstaller.Account = ServiceAccount.LocalSystem;

      serviceInstaller.DisplayName = "TeamSupport Service";
      serviceInstaller.StartType = ServiceStartMode.Automatic;

      //must be the same as what was set in Program's constructor
      serviceInstaller.ServiceName = "TeamSupport";

      this.Installers.Add(processInstaller);
      this.Installers.Add(serviceInstaller);
    }
  }
}
