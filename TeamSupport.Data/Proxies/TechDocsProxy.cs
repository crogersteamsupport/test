using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TechDoc : BaseItem
  {
    public TechDocProxy GetProxy()
    {
      TechDocProxy result = new TechDocProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.AttachmentID = this.AttachmentID;
      result.ProductID = this.ProductID;
      result.OrganizationID = this.OrganizationID;
      result.TechDocID = this.TechDocID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
