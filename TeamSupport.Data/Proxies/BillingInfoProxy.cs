using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class BillingInfoItem : BaseItem
  {
    public BillingInfoItemProxy GetProxy()
    {
      BillingInfoItemProxy result = new BillingInfoItemProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.StoragePrice = this.StoragePrice;
      result.ChatPrice = this.ChatPrice;
      result.BasicPortalPrice = this.BasicPortalPrice;
      result.PortalPrice = this.PortalPrice;
      result.UserPrice = this.UserPrice;
      result.IsAutomatic = this.IsAutomatic;
      result.AddressID = this.AddressID;
      result.CreditCardID = this.CreditCardID;
      result.OrganizationID = this.OrganizationID;
       
      result.NextInvoiceDate = DateTime.SpecifyKind(this.NextInvoiceDateUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
