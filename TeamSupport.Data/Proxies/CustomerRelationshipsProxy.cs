using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CustomerRelationship : BaseItem
  {
    public CustomerRelationshipProxy GetProxy()
    {
      CustomerRelationshipProxy result = new CustomerRelationshipProxy();
      result.CreatorID = this.CreatorID;
      result.RelatedCustomerID = this.RelatedCustomerID;
      result.CustomerID = this.CustomerID;
      result.CustomerRelationshipID = this.CustomerRelationshipID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
