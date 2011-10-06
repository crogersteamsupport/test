using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(PortalOptionProxy))]
  public class PortalOptionProxy
  {
    public PortalOptionProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string PortalHTMLHeader { get; set; }
    [DataMember] public string PortalHTMLFooter { get; set; }
    [DataMember] public bool? UseRecaptcha { get; set; }
    [DataMember] public string FontFamily { get; set; }
    [DataMember] public string FontColor { get; set; }
    [DataMember] public string PageBackgroundColor { get; set; }
    [DataMember] public bool? UseCompanyInBasic { get; set; }
    [DataMember] public bool? CompanyRequiredInBasic { get; set; }
    [DataMember] public bool? HideUserAssignedTo { get; set; }
    [DataMember] public bool? HideGroupAssignedTo { get; set; }
    [DataMember] public int? BasicPortalColumnWidth { get; set; }
    [DataMember] public bool? DisplayGroups { get; set; }
    [DataMember] public string PortalName { get; set; }
    [DataMember] public bool? KBAccess { get; set; }
    [DataMember] public bool? DisplayProducts { get; set; }
    [DataMember] public bool? HonorSupportExpiration { get; set; }
    [DataMember] public bool? HideCloseButton { get; set; }
    [DataMember] public string Theme { get; set; }
    [DataMember] public int? AdvPortalWidth { get; set; }
    [DataMember] public string BasicPortalDirections { get; set; }
    [DataMember] public bool? DeflectionEnabled { get; set; }
    [DataMember] public bool? DisplayForum { get; set; }
    [DataMember] public bool DisplayFooter { get; set; }
    [DataMember] public bool DisplayPortalPhone { get; set; }
    [DataMember] public bool DisplayAdvProducts { get; set; }
    [DataMember] public bool DisplayAdvKB { get; set; }
          
  }
  
  public partial class PortalOption : BaseItem
  {
    public PortalOptionProxy GetProxy()
    {
      PortalOptionProxy result = new PortalOptionProxy();
      result.DisplayAdvKB = this.DisplayAdvKB;
      result.DisplayAdvProducts = this.DisplayAdvProducts;
      result.DisplayPortalPhone = this.DisplayPortalPhone;
      result.DisplayFooter = this.DisplayFooter;
      result.DisplayForum = this.DisplayForum;
      result.DeflectionEnabled = this.DeflectionEnabled;
      result.BasicPortalDirections = this.BasicPortalDirections;
      result.AdvPortalWidth = this.AdvPortalWidth;
      result.Theme = this.Theme;
      result.HideCloseButton = this.HideCloseButton;
      result.HonorSupportExpiration = this.HonorSupportExpiration;
      result.DisplayProducts = this.DisplayProducts;
      result.KBAccess = this.KBAccess;
      result.PortalName = this.PortalName;
      result.DisplayGroups = this.DisplayGroups;
      result.BasicPortalColumnWidth = this.BasicPortalColumnWidth;
      result.HideGroupAssignedTo = this.HideGroupAssignedTo;
      result.HideUserAssignedTo = this.HideUserAssignedTo;
      result.CompanyRequiredInBasic = this.CompanyRequiredInBasic;
      result.UseCompanyInBasic = this.UseCompanyInBasic;
      result.PageBackgroundColor = this.PageBackgroundColor;
      result.FontColor = this.FontColor;
      result.FontFamily = this.FontFamily;
      result.UseRecaptcha = this.UseRecaptcha;
      result.PortalHTMLFooter = this.PortalHTMLFooter;
      result.PortalHTMLHeader = this.PortalHTMLHeader;
      result.OrganizationID = this.OrganizationID;
       
       
       
      return result;
    }	
  }
}
