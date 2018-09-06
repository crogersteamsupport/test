using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class Invoice : BaseItem
  {
    public InvoiceProxy GetProxy()
    {
      InvoiceProxy result = new InvoiceProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.PaymentFailedReason = this.PaymentFailedReason;
      result.PaymentMethod = this.PaymentMethod;
      result.IsPaymentFailed = this.IsPaymentFailed;
      result.IsPaid = this.IsPaid;
      result.TotalAmount = this.TotalAmount;
      result.TotalTaxPrice = this.TotalTaxPrice;
      result.TotalStoragePrice = this.TotalStoragePrice;
      result.TotalPortalPrice = this.TotalPortalPrice;
      result.TotalUserPrice = this.TotalUserPrice;
      result.TaxRate = this.TaxRate;
      result.IsPortalBilled = this.IsPortalBilled;
      result.StoragePrice = this.StoragePrice;
      result.PortalPrice = this.PortalPrice;
      result.UserPrice = this.UserPrice;
      result.ExtraStorageUnits = this.ExtraStorageUnits;
      result.PortalSeats = this.PortalSeats;
      result.UserSeats = this.UserSeats;
      result.CreditCardID = this.CreditCardID;
      result.OrganizationID = this.OrganizationID;
      result.InvoiceID = this.InvoiceID;
       
      result.DateStart = DateTime.SpecifyKind(this.DateStartUtc, DateTimeKind.Utc);
      result.DateEnd = DateTime.SpecifyKind(this.DateEndUtc, DateTimeKind.Utc);
      result.DateBilled = DateTime.SpecifyKind(this.DateBilledUtc, DateTimeKind.Utc);
      result.DateDue = DateTime.SpecifyKind(this.DateDueUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
