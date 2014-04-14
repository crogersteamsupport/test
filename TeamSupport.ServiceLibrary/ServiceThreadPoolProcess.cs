using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamSupport.ServiceLibrary
{
  public abstract class ServiceThreadPoolProcess: ServiceThread
  {
    protected int? _threadPosition = null;
    protected int _id;

    public ServiceThreadPoolProcess() { 
      IsLoop = false; 
    }

    public override string GetLogFileCategory()
    {
      return _threadPosition == null ? ServiceName : ServiceName + " - " + _threadPosition.ToString();
    }

    public void Start(int id, int threadPosition)
    {
      _threadPosition = threadPosition;
      _id = id;
      base.Start();
    }

    public abstract void ReleaseAllLocks();

    public abstract int GetNextID(int threadPosition);

  }
}
