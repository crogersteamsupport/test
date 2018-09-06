using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CRMLinkError : BaseItem
  {
    public CRMLinkErrorProxy GetProxy()
    {
      CRMLinkErrorProxy result = new CRMLinkErrorProxy();
      result.IsCleared = this.IsCleared;
      result.ErrorCount = this.ErrorCount;
      result.ErrorMessage = this.ErrorMessage;
      result.OperationType = this.OperationType;
      result.Exception = this.Exception;
      result.ObjectData = this.ObjectData;
      result.ObjectFieldName = this.ObjectFieldName;
      result.ObjectID = this.ObjectID;
      result.ObjectType = this.ObjectType;
      result.Orientation = this.Orientation;
      result.CRMType = this.CRMType;
      result.OrganizationID = this.OrganizationID;
      result.CRMLinkErrorID = this.CRMLinkErrorID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
