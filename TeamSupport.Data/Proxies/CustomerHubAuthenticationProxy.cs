using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CustomerHubAuthenticationItem : BaseItem
  {
    public CustomerHubAuthenticationItemProxy GetProxy()
    {
      CustomerHubAuthenticationItemProxy result = new CustomerHubAuthenticationItemProxy();
      result.AnonymousChatAccess = this.AnonymousChatAccess;
      result.ModifierID = this.ModifierID;
      result.RequireTermsAndConditions = this.RequireTermsAndConditions;
      result.HonorSupportExpiration = this.HonorSupportExpiration;
      result.HonorServiceAgreementExpirationDate = this.HonorServiceAgreementExpirationDate;
      result.AnonymousTicketAccess = this.AnonymousTicketAccess;
      result.AnonymousProductAccess = this.AnonymousProductAccess;
      result.AnonymousKBAccess = this.AnonymousKBAccess;
      result.AnonymousWikiAccess = this.AnonymousWikiAccess;
      result.AnonymousHubAccess = this.AnonymousHubAccess;
      result.RequestGroupType = this.RequestGroupType;
      result.RequestTicketType = this.RequestTicketType;
      result.EnableSSO = this.EnableSSO;
      result.EnableRequestAccess = this.EnableRequestAccess;
      result.EnableSelfRegister = this.EnableSelfRegister;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubAuthenticationID = this.CustomerHubAuthenticationID;
       
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
