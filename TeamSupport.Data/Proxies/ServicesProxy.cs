using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Service : BaseItem
  {
    public ServiceProxy GetProxy()
    {
      ServiceProxy result = new ServiceProxy();
      result.HealthMaxMinutes = this.HealthMaxMinutes;
      result.NameSpace = this.NameSpace;
      result.AutoStart = this.AutoStart;
      result.AssemblyName = this.AssemblyName;
      result.RunTimeMax = this.RunTimeMax;
      result.RunTimeAvg = this.RunTimeAvg;
      result.RunCount = this.RunCount;
      result.ErrorCount = this.ErrorCount;
      result.LastError = this.LastError;
      result.LastResult = this.LastResult;
      result.Interval = this.Interval;
      result.Enabled = this.Enabled;
      result.Name = this.Name;
      result.ServiceID = this.ServiceID;
       
       
      result.HealthTime = this.HealthTimeUtc == null ? this.HealthTimeUtc : DateTime.SpecifyKind((DateTime)this.HealthTimeUtc, DateTimeKind.Utc); 
      result.LastEndTime = this.LastEndTimeUtc == null ? this.LastEndTimeUtc : DateTime.SpecifyKind((DateTime)this.LastEndTimeUtc, DateTimeKind.Utc); 
      result.LastStartTime = this.LastStartTimeUtc == null ? this.LastStartTimeUtc : DateTime.SpecifyKind((DateTime)this.LastStartTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
