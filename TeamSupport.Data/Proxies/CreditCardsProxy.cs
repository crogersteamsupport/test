using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CreditCard : BaseItem
  {
    public CreditCardProxy GetProxy()
    {
      CreditCardProxy result = new CreditCardProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.NameOnCard = this.NameOnCard;
      result.SecurityCode = this.SecurityCode;
      result.CardNumber = this.CardNumber;
      result.CreditCardType = this.CreditCardType;
      result.DisplayNumber = this.DisplayNumber;
      result.OrganizationID = this.OrganizationID;
      result.CreditCardID = this.CreditCardID;
       
      result.ExpirationDate = DateTime.SpecifyKind(this.ExpirationDateUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModfied = DateTime.SpecifyKind(this.DateModfiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
