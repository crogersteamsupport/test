using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class TicketType : BaseItem
  {
    public TicketTypeProxy GetProxy()
    {
      TicketTypeProxy result = new TicketTypeProxy();
      result.IsActive = this.IsActive;
      result.ProductFamilyID = this.ProductFamilyID;
      result.IconUrl = this.IconUrl;
      //result.ModifierID = this.ModifierID;
      //result.CreatorID = this.CreatorID;
      //result.OrganizationID = this.OrganizationID;
      result.Position = this.Position;
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.TicketTypeID = this.TicketTypeID;
       
       
       
      return result;
    }	
  }
}
