using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class ContactsViewItem : BaseItem
  {
    public ContactsViewItemProxy GetProxy()
    {
      ContactsViewItemProxy result = new ContactsViewItemProxy();
      result.BlockInboundEmail = this.BlockInboundEmail;
      result.PortalViewOnly = this.PortalViewOnly;
      result.LinkedIn = this.LinkedIn;
      result.PortalLimitOrgTickets = this.PortalLimitOrgTickets;
      result.OrganizationActive = this.OrganizationActive;
      result.NeedsIndexing = this.NeedsIndexing;
      result.SalesForceID = this.SalesForceID;
      result.CryptedPassword = this.CryptedPassword;
      result.OrganizationParentID = this.OrganizationParentID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.LastVersion = this.LastVersion;
      result.Organization = this.Organization;
      result.OrganizationID = this.OrganizationID;
      result.InOfficeComment = this.InOfficeComment;
      result.InOffice = this.InOffice;
      result.PrimaryGroupID = this.PrimaryGroupID;
      result.IsPortalUser = this.IsPortalUser;
      result.IsPasswordExpired = this.IsPasswordExpired;
      result.IsFinanceAdmin = this.IsFinanceAdmin;
      result.IsSystemAdmin = this.IsSystemAdmin;
      result.MarkDeleted = this.MarkDeleted;
      result.IsActive = this.IsActive;
      result.Title = this.Title;
      result.LastName = this.LastName;
      result.MiddleName = this.MiddleName;
      result.UserID = this.UserID;
      result.FirstName = this.FirstName;
      result.Email = this.Email;
       
      result.LastLogin = DateTime.SpecifyKind(this.LastLoginUtc, DateTimeKind.Utc);
      result.LastActivity = DateTime.SpecifyKind(this.LastActivityUtc, DateTimeKind.Utc);
      result.ActivatedOn = DateTime.SpecifyKind(this.ActivatedOnUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.OrganizationSAExpirationDate = this.OrganizationSAExpirationDateUtc == null ? this.OrganizationSAExpirationDateUtc : DateTime.SpecifyKind((DateTime)this.OrganizationSAExpirationDateUtc, DateTimeKind.Utc); 
      result.DeactivatedOn = this.DeactivatedOnUtc == null ? this.DeactivatedOnUtc : DateTime.SpecifyKind((DateTime)this.DeactivatedOnUtc, DateTimeKind.Utc); 
      result.LastPing = this.LastPingUtc == null ? this.LastPingUtc : DateTime.SpecifyKind((DateTime)this.LastPingUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
