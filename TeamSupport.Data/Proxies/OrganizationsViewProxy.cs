using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class OrganizationsViewItem : BaseItem
  {
    public OrganizationsViewItemProxy GetProxy()
    {
      OrganizationsViewItemProxy result = new OrganizationsViewItemProxy();

      result.NeedsIndexing = this.NeedsIndexing;
      result.SupportHoursRemaining = this.SupportHoursRemaining;
      result.SupportHoursUsed = this.SupportHoursUsed;
      result.DefaultSupportUserID = this.DefaultSupportUserID;
      result.DefaultSupportGroupID = this.DefaultSupportGroupID;
      result.DefaultWikiArticleID = this.DefaultWikiArticleID;
      result.SlaLevelID = this.SlaLevelID;
      result.PortalGuid = this.PortalGuid;
      result.CRMLinkID = this.CRMLinkID;
      result.SlaName = this.SlaName;
      result.LastModifiedBy = this.LastModifiedBy;
      result.CreatedBy = this.CreatedBy;
      result.HasPortalAccess = this.HasPortalAccess;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ParentID = this.ParentID;
      result.PrimaryContact = this.PrimaryContact;
      result.PrimaryUserID = this.PrimaryUserID;
      result.InActiveReason = this.InActiveReason;
      result.IsActive = this.IsActive;
      result.Website = (this.Website);
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.OrganizationID = this.OrganizationID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.SAExpirationDate = this.SAExpirationDateUtc == null ? this.SAExpirationDateUtc : DateTime.SpecifyKind((DateTime)this.SAExpirationDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
