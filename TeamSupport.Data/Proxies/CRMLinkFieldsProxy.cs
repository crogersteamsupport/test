using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CRMLinkField : BaseItem
  {
    public CRMLinkFieldProxy GetProxy()
    {
      CRMLinkFieldProxy result = new CRMLinkFieldProxy();
      result.TSFieldName = this.TSFieldName;
      result.CustomFieldID = this.CustomFieldID;
      result.CRMFieldName = this.CRMFieldName;
      result.CRMObjectName = this.CRMObjectName;
      result.CRMLinkID = this.CRMLinkID;
      result.CRMFieldID = this.CRMFieldID;
       
       
       
      return result;
    }	
  }
}
