using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;


namespace TeamSupport.ServiceLibrary
{
  public class MurocUpdater : ServiceThread
  {
    public MurocUpdater() { }

    public override string ServiceName
    {
      get { return "muroc"; }
    }

    public override void Run()
    {
      try
      {
        Organizations murocCustomers = new Organizations(LoginUser);
        Organizations tsCustomers = new Organizations(LoginUser);
        Users murocContacts = new Users(LoginUser);
        Users tsContacts = new Users(LoginUser);

        murocCustomers.LoadByParentID(1078, false);
        tsCustomers.LoadByParentID(1, false);
        murocContacts.LoadContacts(1078, false);
        tsContacts.LoadContacts(1, false);

        SyncOrganizations(murocCustomers, tsCustomers, murocContacts, tsContacts);

        murocCustomers.LoadByParentID(1078, false);
        tsCustomers.LoadByParentID(1, false);
        SyncUsers(murocCustomers, tsCustomers, murocContacts, tsContacts);
        murocContacts.LoadContacts(1078, false);
        tsContacts.LoadContacts(1, false);

        SyncOrganizations(murocCustomers, tsCustomers, murocContacts, tsContacts);

      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(LoginUser, ex, "Muroc Updater", "Sync");
      }
    }

    private void SyncOrganizations(Organizations murocCustomers, Organizations tsCustomers, Users murocContacts, Users tsContacts)
    {
      foreach (Organization tsCustomer in tsCustomers)
      {
        if (IsStopped) break;
        Organization murocCustomer = murocCustomers.FindByImportID(tsCustomer.OrganizationID.ToString());
        if (murocCustomer != null)
        {
          UpdateOrganization(tsCustomer, murocCustomer, murocContacts);
        }
        else
        {
          murocCustomer = murocCustomers.FindByName(tsCustomer.Name);
          if (murocCustomer != null)
          {
            UpdateOrganization(tsCustomer, murocCustomer, murocContacts);
          }
          else
          {
            InsertOrganization(tsCustomer, murocCustomers, murocContacts);
          }
        }
        System.Threading.Thread.Sleep(0);
      }
    }

    private void UpdateOrganization(Organization source, Organization target, Users murocContacts)
    {
      int? targetUserID = target.PrimaryUserID;
      if (targetUserID != null)
      {
        User user = murocContacts.FindByUserID((int)targetUserID);
        if (user != null)
        { 
          try 
	        {
            targetUserID = int.Parse(user.ImportID);
	        }
	        catch (Exception)
	        {
	        }
        }
      }
      
      if (target.Description != source.Description ||
          target.DateCreated != source.DateCreated ||
          target.IsActive != source.IsActive ||
          target.Website != source.Website ||
          targetUserID != source.PrimaryUserID ||
          target.Name != source.Name
          )
      {
        target.Description = source.Description;
        target.ImportID = source.OrganizationID.ToString();
        target.DateCreated = source.DateCreated;
        target.IsActive = source.IsActive;
        target.Website = source.Website;
        target.Name = source.Name;
        if (source.PrimaryUserID != null)
        {
          User user = murocContacts.FindByImportID(source.PrimaryUserID.ToString());
          if (user != null) target.PrimaryUserID = user.UserID;
        }
        else
        {
          target.PrimaryUserID = null; 
        }
        target.Collection.Save();
      }
    
    }

    private void InsertOrganization(Organization source, Organizations organizations, Users murocContacts)
    {
      Organization organization = (new Organizations(LoginUser)).AddNewOrganization();
      organization.Description = source.Description;
      organization.ImportID = source.OrganizationID.ToString();
      organization.DateCreated = source.DateCreated;
      organization.IsActive = source.IsActive;
      organization.Website = source.Website;
      organization.Name = source.Name;
      organization.HasPortalAccess = true;
      organization.IsAdvancedPortal = false;
      organization.IsBasicPortal = false;
      organization.ParentID = 1078;

      if (source.PrimaryUserID != null)
      {
        User user = murocContacts.FindByImportID(source.PrimaryUserID.ToString());
        if (user != null) organization.PrimaryUserID = user.UserID;
      }
      else
      {
        organization.PrimaryUserID = null;
      }

      organization.Collection.Save();

      AddProducts(organization.OrganizationID);
    }

    private void AddProducts(int organizationID)
    {
      Products products = new Products(LoginUser);
      products.AddCustomer(organizationID, 219);
      products.AddCustomer(organizationID, 233);
      products.AddCustomer(organizationID, 234);
      products.AddCustomer(organizationID, 1068);
      products.Save();
    }

    private void SyncUsers(Organizations murocCustomers, Organizations tsCustomers, Users murocContacts, Users tsContacts)
    {
      foreach (User tsContact in tsContacts)
      {
        if (IsStopped) break;
   
        User murocContact = murocContacts.FindByImportID(tsContact.UserID.ToString());
        if (murocContact != null && CompareOrganizationIDs(tsContact.OrganizationID, murocContact.OrganizationID, murocCustomers))
        {
          UpdateContact(tsContact, murocContact, murocCustomers);
        }
        else
        {
          murocContact = murocContacts.FindByName(tsContact.FirstName, tsContact.LastName);
          if (murocContact != null && CompareOrganizationIDs(tsContact.OrganizationID, murocContact.OrganizationID, murocCustomers))
          {
            UpdateContact(tsContact, murocContact, murocCustomers);
          }
          else
          {
            murocContact = murocContacts.FindByEmail(tsContact.Email);
            if (murocContact != null && CompareOrganizationIDs(tsContact.OrganizationID, murocContact.OrganizationID, murocCustomers))
            {
              UpdateContact(tsContact, murocContact, murocCustomers);
            }
            else
            {
              InsertContact(tsContact, murocContacts, murocCustomers);
            }
          }
        }
        System.Threading.Thread.Sleep(0);
      }
    
    }

    private bool CompareOrganizationIDs(int tsID, int murocID, Organizations murocCustomers)
    {
      Organization organization = murocCustomers.FindByImportID(tsID.ToString());
      if (organization == null) return false;
      return murocID == organization.OrganizationID;
    }

    private bool CompareUserIDs(int? tsID, int? murocID, Users murocContacts)
    {
      if (tsID == null || murocID == null)
      {
        return tsID == murocID;
      }
      
      User user = murocContacts.FindByImportID(tsID.ToString());
      if (user == null) return false;
      return murocID == user.UserID;
    }

    private void UpdateContact(User source, User target, Organizations murocCustomers)
    {
      if (target.DateCreated != source.DateCreated ||
          target.Email != source.Email ||
          target.FirstName != source.FirstName ||
          target.IsActive != source.IsActive ||
          target.LastName != source.LastName ||
          target.MiddleName != source.MiddleName ||
          target.CryptedPassword != source.CryptedPassword ||
          target.Title != source.Title)
      {
        target.DateCreated = source.DateCreated;
        target.ImportID = source.UserID.ToString();
        target.Email = source.Email;
        target.FirstName = source.FirstName;
        target.IsActive = source.IsActive;
        target.LastName = source.LastName;
        target.MiddleName = source.MiddleName;
        target.CryptedPassword = source.CryptedPassword;
        target.IsPortalUser = true;
        target.Title = source.Title;
        target.Collection.Save();
      }

    }

    private void InsertContact(User source, Users users, Organizations murocCustomers)
    {
      Organization organization = murocCustomers.FindByImportID(source.OrganizationID.ToString());
      if (organization == null) return;

      User target = (new Users(LoginUser)).AddNewUser();
      target.DateCreated = source.DateCreated;
      target.ImportID = source.UserID.ToString();
      target.Email = source.Email;
      target.FirstName = source.FirstName;
      target.IsActive = source.IsActive;
      target.LastName = source.LastName;
      target.MiddleName = source.MiddleName;
      target.Title = source.Title;
      target.ReceiveTicketNotifications = true;
      target.OrganizationID = organization.OrganizationID;
      target.CryptedPassword = source.CryptedPassword;
      target.IsPortalUser = true;
      target.Collection.Save();
    }



  }
}
