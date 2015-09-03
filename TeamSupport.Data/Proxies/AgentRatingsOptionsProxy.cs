using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(AgentRatingsOptionProxy))]
  public class AgentRatingsOptionProxy
  {
    public AgentRatingsOptionProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string PositiveRatingText { get; set; }
    [DataMember] public string NeutralRatingText { get; set; }
    [DataMember] public string NegativeRatingText { get; set; }
    [DataMember] public string PositiveImage { get; set; }
    [DataMember] public string NeutralImage { get; set; }
    [DataMember] public string NegativeImage { get; set; }
    [DataMember] public string RedirectURL { get; set; }
    [DataMember] public string ExternalPageLink { get; set; }
          
  }
  
  public partial class AgentRatingsOption : BaseItem
  {
    public AgentRatingsOptionProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      AgentRatingsOptionProxy result = new AgentRatingsOptionProxy();
      result.ExternalPageLink = sanitizer.Sanitize(this.ExternalPageLink);
      result.RedirectURL = sanitizer.Sanitize(this.RedirectURL);
      result.NegativeImage = sanitizer.Sanitize(this.NegativeImage);
      result.NeutralImage = sanitizer.Sanitize(this.NeutralImage);
      result.PositiveImage = sanitizer.Sanitize(this.PositiveImage);
      result.NegativeRatingText = sanitizer.Sanitize(this.NegativeRatingText);
      result.NeutralRatingText = sanitizer.Sanitize(this.NeutralRatingText);
      result.PositiveRatingText = sanitizer.Sanitize(this.PositiveRatingText);
      result.OrganizationID = this.OrganizationID;
       
       
       
      return result;
    }	
  }
}
