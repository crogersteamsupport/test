using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketTypesViewItem : BaseItem
  {
    public TicketTypesViewItemProxy GetProxy()
    {
      TicketTypesViewItemProxy result = new TicketTypesViewItemProxy();
      result.IsActive = this.IsActive;
      result.ProductFamilyName = (this.ProductFamilyName);
      result.ProductFamilyID = this.ProductFamilyID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.IconUrl = this.IconUrl;
      result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.TicketTypeID = this.TicketTypeID;
      result.ExcludeFromCDI = this.ExcludeFromCDI;


      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
