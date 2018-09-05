using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class PhoneOption : BaseItem
  {
    public PhoneOptionProxy GetProxy()
    {
      PhoneOptionProxy result = new PhoneOptionProxy();
      result.WelcomeAudioURL = this.WelcomeAudioURL;
      result.AccountToken = this.AccountToken;
      result.AccountSID = this.AccountSID;
      result.PhoneActive = this.PhoneActive;
      result.OrganizationID = this.OrganizationID;
      result.PhoneOptionID = this.PhoneOptionID;
       
       
       
      return result;
    }	
  }
}
