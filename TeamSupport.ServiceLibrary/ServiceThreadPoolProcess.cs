using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.ServiceLibrary
{
  public abstract class ServiceThreadPoolProcess: ServiceThread
  {
    private int _threadPosition;
    public ServiceThreadPoolProcess(int threadPosition)
    {
      _threadPosition = threadPosition;
      IsLoop = false;
    }

    public override string GetLogFileCategory()
    {
      return ServiceName + " - " + _threadPosition.ToString();
    }
  }
}
