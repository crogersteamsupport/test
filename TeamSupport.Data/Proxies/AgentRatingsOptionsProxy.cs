using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class AgentRatingsOption : BaseItem
  {
    public AgentRatingsOptionProxy GetProxy()
    {
      AgentRatingsOptionProxy result = new AgentRatingsOptionProxy();
      result.ExternalPageLink = this.ExternalPageLink;
      result.RedirectURL = this.RedirectURL;
      result.NegativeImage = this.NegativeImage;
      result.NeutralImage = this.NeutralImage;
      result.PositiveImage = this.PositiveImage;
      result.NegativeRatingText = this.NegativeRatingText;
      result.NeutralRatingText = this.NeutralRatingText;
      result.PositiveRatingText = this.PositiveRatingText;
      result.OrganizationID = this.OrganizationID;
       
       
       
      return result;
    }	
  }
}
