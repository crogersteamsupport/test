using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamSupport.Data;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;
using LumenWorks.Framework.IO.Csv;
using System.Data;
using System.Text.RegularExpressions;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  public class ImportProcessor : ServiceThread
  {
    const int BULK_LIMIT = 1000;

    private int _organizationID;
    private LoginUser _importUser;
    private CsvReader _csv;
    private SortedList<string, string> _map;
    private string[] _headers;

    public override void Run()
    {
      Imports imports = new Imports(LoginUser);
      imports.LoadWaiting();

      try
      {
        if (!imports.IsEmpty)
        {
          ProcessImport(imports[0]);
        }
        UpdateHealth();
      }
      catch (Exception ex)
      {
        Logs.WriteException(ex);
        ExceptionLogs.LogException(LoginUser, ex, "ImportProcessor"); 
      }
      finally
      {
        ClearEmails();

      }
    }

   private void UpdateImportCount(Import import, int count)
    {
      import.CompletedRows = count;
      import.Collection.Save();
    }

    private void ProcessImport(Import import)
    {
      Logs.WriteLine();
      Logs.WriteEvent("***********************************************************************************");
      Logs.WriteEvent("Processing Import  ImportID: " + import.ImportID.ToString());
      Logs.WriteData(import.Row);
      Logs.WriteLine();
      Logs.WriteEvent("***********************************************************************************");
      _importUser = new Data.LoginUser(LoginUser.ConnectionString, -5, import.OrganizationID, null);


      //string csvFile = "U:\\Development\\Imports\\TestFiles\test.csv"; // Path.Combine(path, import.FileName);
      string csvFile = Path.Combine(AttachmentPath.GetPath(_importUser, import.OrganizationID, AttachmentPath.Folder.Imports), import.FileName);
      _organizationID = import.OrganizationID;

      import.TotalRows = GetTotalRows(csvFile);
      import.CompletedRows = 0;
      import.IsRunning = true;
      import.DateStarted = DateTime.UtcNow;
      import.Collection.Save();

      _map = new SortedList<string, string>();
      // load maps
      using (_csv = new CsvReader(new StreamReader(csvFile), true))
      {
        _headers = _csv.GetFieldHeaders();
        switch (import.RefType)
        {
          case ReferenceType.Actions:
            ImportActions(import);
            break;
          case ReferenceType.Assets:
            ImportAssets(import);
            break;
          case ReferenceType.Organizations:
            ImportCompanies(import);
            break;
          case ReferenceType.Contacts:
            ImportContacts(import);
            break;
          case ReferenceType.CustomFieldPickList:
            ImportCustomFieldPickList(import);
            break;
          default:
            Logs.WriteEvent("ERROR: Unknown Reference Type");
            break;
        }
      }

      import.IsDone = true;
      import.DateEnded = DateTime.UtcNow;
      import.Collection.Save();

    }

    private int GetTotalRows(string csvFile)
    {
      using (CsvReader csv = new CsvReader(new StreamReader(csvFile), true))
      {
        while (csv.ReadNextRecord()) ;
        return (int)csv.CurrentRecordIndex + 1;
      }
    }

    private void ImportActions(Import import)
    {
      SortedList<string, int> userList = GetUserAndContactList();
      SortedList<string, int> ticketList = GetTicketList();

      ActionTypes actionTypes = new ActionTypes(_importUser);
      actionTypes.LoadAllPositions(_organizationID);

      Actions actions = new Actions(_importUser);
      int count = 0;

      while (_csv.ReadNextRecord())
      {
        int ticketID = ReadInt("TicketID");

        if (!ticketList.ContainsValue(ticketID))
        {
          //_log.AppendError(row, "Action skipped due to missing ticket");
          continue;
        }

        Actions existingAction = new Actions(_importUser);
        TeamSupport.Data.Action action = null;
        bool isUpdate = false;

        //int actionID = ReadInt("ActionID");
        //if (actionID != 0)
        //{
        //  existingAction.LoadByActionID(actionID);
        //  if (existingAction.Count > 0)
        //  {
        //    action = existingAction[0];
        //    isUpdate = true;
        //  }
        //}

        //if (action == null)
        //{
          string importID = ReadString("ImportID");
          if (importID != string.Empty)
          {
            existingAction = new Actions(_importUser);
            existingAction.LoadByImportID(importID, _organizationID);
            if (existingAction.Count == 1)
            {
              action = existingAction[0];
              isUpdate = true;
            }
            else if (existingAction.Count > 1)
            {
              // More than one action matching the importID was found
              continue;
            }
          }
        //}

        if (action == null)
        {
          action = actions.AddNewAction();
        }

        string actionType = ReadString("ActionType");
        action.SystemActionTypeID = GetSystemActionTypeID(actionType);
        action.ActionTypeID = GetActionTypeID(actionTypes, actionType);

        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID)) {
          if (!userList.ContainsValue(creatorID)){
            creatorID = -2;
          }
        }
        action.CreatorID = creatorID;

        string desc = ConvertHtmlLineBreaks(ReadString("Description"));
        action.Description = desc;

        action.DateCreated = ReadDate("DateCreated", DateTime.UtcNow);
        action.DateModified = DateTime.UtcNow;
        action.DateStarted = ReadDateNull("DateStarted");
        action.ActionSource = "Import";
        action.IsVisibleOnPortal = ReadBool("VisibleOnPortal");
        action.ModifierID = -2;
        action.Name = "";
        action.TicketID = ticketID;
        action.ImportID = ReadString("ImportID");
        action.TimeSpent = ReadIntNull("TimeSpent");

        action.Pinned = ReadBool("IsPinned");

        if (isUpdate)
        {
          existingAction.Save();
          // Add updated rows column as completed rows will reflect only adds
        }
        count++;

        if (count % BULK_LIMIT == 0)
        {
          actions.BulkSave();
          actions = new Actions(_importUser);
          UpdateImportCount(import, count);
        }
      }
      actions.BulkSave();
      UpdateImportCount(import, count);
      // _log.AppendMessage(count.ToString() + " Actions Imported.");
    }

    private void ImportAssets(Import import)
    {
      Products products = new Products(_importUser);
      products.LoadByOrganizationID(_organizationID);
      //IdList productIDs = GetIdList(products);

      ProductVersions productVersions = new ProductVersions(_importUser);
      productVersions.LoadByParentOrganizationID(_organizationID);

      Assets assets = new Assets(_importUser);
      //int orgCount = 0;
      //int prodCount = 0;

      SortedList<string, int> userList = GetUserAndContactList();

      int count = 0;
      while (_csv.ReadNextRecord())
      {
        //_currentRow = row;
        //string organizationID = row["AssignedTo"].ToString().Trim().ToLower();

        Product product = null;
        string productName = ReadString("Product");
        if (productName != string.Empty)
        {
          product = products.FindByName(productName);
        }

        if (product == null)
        {
          continue;
        }

        Assets existingAsset = new Assets(_importUser);
        Asset asset = null;
        bool isUpdate = false;

        //int assetID = ReadInt("AssetID");
        //if (assetID != 0)
        //{
        //  existingAsset.LoadByAssetID(assetID);
        //  if (existingAsset.Count > 0)
        //  {
        //    asset = existingAsset[0];
        //    isUpdate = true;
        //  }
        //}

        //if (asset == null)
        //{
          string importID = ReadString("ImportID");
          if (importID != string.Empty)
          {
            existingAsset = new Assets(_importUser);
            existingAsset.LoadByImportID(importID, _organizationID);
            if (existingAsset.Count == 1)
            {
              asset = existingAsset[0];
              isUpdate = true;
            }
            else if (existingAsset.Count > 1)
            {
              // Log more than one asset already exists in the database with given importID
              continue;
            }
          }
        //}

        string location = "2";
        switch (ReadString("Location").ToLower().Trim())
        {
          case "assigned": location = "1"; break;
          case "warehouse": location = "2"; break;
          case "junkyard": location = "3"; break;
          default:
            break;
        }

        Assets newAssignedAsset = new Assets(_importUser);
        if (asset == null)
        {
          if (location == "1")
          {
            asset = newAssignedAsset.AddNewAsset();
          }
          else
          {
            asset = assets.AddNewAsset();
          }
        }

        asset.OrganizationID = _organizationID;
        asset.SerialNumber = ReadString("SerialNumber");
        asset.Name = ReadString("Name");
        string oldLocation = asset.Location;
        asset.Location = location;
        asset.Notes = ReadString("Notes");
        asset.ProductID = product.ProductID;
        asset.WarrantyExpiration = (DateTime)ReadDate("WarrantyExpiration", DateTime.UtcNow);
        asset.DateCreated = (DateTime)ReadDate("DateCreated", DateTime.UtcNow);
        asset.DateModified = DateTime.UtcNow;

        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID))
        {
          if (!userList.ContainsValue(creatorID))
          {
            creatorID = -2;
          }
        }
        asset.CreatorID = creatorID;
        asset.ModifierID = -2;
        asset.SubPartOf = null;
        //asset.Status = this is a deprecated field
        asset.ImportID = ReadString("ImportID");

        ProductVersion productVersion = null;
        string productVersionNumber = ReadString("ProductVersion");
        if (productVersionNumber != string.Empty)
        {
          productVersion = productVersions.FindByVersionNumber(productVersionNumber, product.ProductID);
          if (productVersion != null)
          {
            asset.ProductVersionID = productVersion.ProductVersionID;
          }
          else
          {
            // Log no product version found without continuing
          }
        }

        if (asset.Location == "1" && (!isUpdate || oldLocation != "1"))
        {
          string nameOfCompanyAssignedTo = ReadString("NameOfCompanyAssignedTo");
          if (!string.IsNullOrEmpty(nameOfCompanyAssignedTo))
          {
            Organizations companyAssignedTo = new Organizations(_importUser);
            companyAssignedTo.LoadByOrganizationNameActive(nameOfCompanyAssignedTo, _organizationID);
            if (companyAssignedTo.Count == 1)
            {
              string emailOfContactAssignedTo = ReadString("EmailOfContactAssignedTo");
              if (!string.IsNullOrEmpty(emailOfContactAssignedTo))
              {
                Users contactAssignedTo = new Users(_importUser);
                contactAssignedTo.LoadByEmail(emailOfContactAssignedTo, companyAssignedTo[0].OrganizationID);
                if (contactAssignedTo.Count == 1)
                {
                  DateTime? dateShipped = ReadDateNull("DateShipped");
                  string shippingMethod = ReadString("ShippingMethod");
                  if (dateShipped == null || shippingMethod == string.Empty)
                  {
                    // Log dateShipped and shippingMethod are required
                    continue;
                  }

                  DateTime validDateShipped = (DateTime)dateShipped;

                  if (!isUpdate)
                  {
                    newAssignedAsset.Save();
                  }

                  AssetHistory assetHistory = new AssetHistory(_importUser);
                  AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                  DateTime now = DateTime.UtcNow;

                  assetHistoryItem.AssetID = asset.AssetID;
                  assetHistoryItem.OrganizationID = _organizationID;
                  assetHistoryItem.ActionTime = now;
                  assetHistoryItem.ActionDescription = "Asset Shipped on " + validDateShipped.Month.ToString() + "/" + validDateShipped.Day.ToString() + "/" + validDateShipped.Year.ToString();
                  assetHistoryItem.ShippedFrom = _organizationID;
                  assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                  assetHistoryItem.ShippedTo = contactAssignedTo[0].UserID;
                  assetHistoryItem.TrackingNumber = ReadString("TrackingNumber");
                  assetHistoryItem.ShippingMethod = shippingMethod;
                  assetHistoryItem.ReferenceNum = ReadString("ReferenceNumber");
                  assetHistoryItem.Comments = ReadString("Comments");

                  assetHistoryItem.DateCreated = now;
                  assetHistoryItem.Actor = -2;
                  assetHistoryItem.RefType = (int)ReferenceType.Contacts;
                  assetHistoryItem.DateModified = now;
                  assetHistoryItem.ModifierID = -2;

                  assetHistory.Save();

                  AssetAssignments assetAssignments = new AssetAssignments(_importUser);
                  AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                  assetAssignment.HistoryID = assetHistoryItem.HistoryID;

                  assetAssignments.Save();

                  string description = String.Format("Assigned asset to {0}.", contactAssignedTo[0].FirstLastName);
                  ActionLogs.AddActionLog(_importUser, ActionLogType.Update, ReferenceType.Assets, asset.AssetID, description);
                }
                else if (contactAssignedTo.Count > 1)
                {
                  // Log more than one email matching contact found
                  continue;
                }
                else
                {
                  // Log no email matching contact found
                  continue;
                }
              }
              else
              {
                DateTime? dateShipped = ReadDateNull("DateShipped");
                string shippingMethod = ReadString("ShippingMethod");
                if (dateShipped == null || shippingMethod == string.Empty)
                {
                  // Log dateShipped and shippingMethod are required
                  continue;
                }

                DateTime validDateShipped = (DateTime)dateShipped;

                if (!isUpdate)
                {
                  newAssignedAsset.Save();
                }

                AssetHistory assetHistory = new AssetHistory(_importUser);
                AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                DateTime now = DateTime.UtcNow;

                assetHistoryItem.AssetID = asset.AssetID;
                assetHistoryItem.OrganizationID = _organizationID;
                assetHistoryItem.ActionTime = now;
                assetHistoryItem.ActionDescription = "Asset Shipped on " + validDateShipped.Month.ToString() + "/" + validDateShipped.Day.ToString() + "/" + validDateShipped.Year.ToString();
                assetHistoryItem.ShippedFrom = _organizationID;
                assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                assetHistoryItem.ShippedTo = companyAssignedTo[0].OrganizationID;
                assetHistoryItem.TrackingNumber = ReadString("TrackingNumber");
                assetHistoryItem.ShippingMethod = shippingMethod;
                assetHistoryItem.ReferenceNum = ReadString("ReferenceNumber");
                assetHistoryItem.Comments = ReadString("Comments");

                assetHistoryItem.DateCreated = now;
                assetHistoryItem.Actor = -2;
                assetHistoryItem.RefType = (int)ReferenceType.Organizations;
                assetHistoryItem.DateModified = now;
                assetHistoryItem.ModifierID = -2;

                assetHistory.Save();

                AssetAssignments assetAssignments = new AssetAssignments(_importUser);
                AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                assetAssignment.HistoryID = assetHistoryItem.HistoryID;

                assetAssignments.Save();

                string description = String.Format("Assigned asset to {0}.", companyAssignedTo[0].Name);
                ActionLogs.AddActionLog(_importUser, ActionLogType.Update, ReferenceType.Assets, asset.AssetID, description);
              }
            }
            else if (companyAssignedTo.Count > 1)
            {
              // Log more than one name matching company found
              continue;
            }
            else
            {
              // Log no name matching company found
              continue;
            }
          }
          else
          {
            string emailOfContactAssignedTo = ReadString("EmailOfContactAssignedTo");
            if (!string.IsNullOrEmpty(emailOfContactAssignedTo))
            {
              Users contactAssignedTo = new Users(_importUser);
              contactAssignedTo.LoadByEmail(_organizationID, emailOfContactAssignedTo);
              if (contactAssignedTo.Count == 1)
              {
                DateTime? dateShipped = ReadDateNull("DateShipped");
                string shippingMethod = ReadString("ShippingMethod");
                if (dateShipped == null || shippingMethod == string.Empty)
                {
                  // Log dateShipped and shippingMethod are required
                  continue;
                }

                DateTime validDateShipped = (DateTime)dateShipped;

                if (!isUpdate)
                {
                  newAssignedAsset.Save();
                }

                AssetHistory assetHistory = new AssetHistory(_importUser);
                AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                DateTime now = DateTime.UtcNow;

                assetHistoryItem.AssetID = asset.AssetID;
                assetHistoryItem.OrganizationID = _organizationID;
                assetHistoryItem.ActionTime = now;
                assetHistoryItem.ActionDescription = "Asset Shipped on " + validDateShipped.Month.ToString() + "/" + validDateShipped.Day.ToString() + "/" + validDateShipped.Year.ToString();
                assetHistoryItem.ShippedFrom = _organizationID;
                assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                assetHistoryItem.ShippedTo = contactAssignedTo[0].UserID;
                assetHistoryItem.TrackingNumber = ReadString("TrackingNumber");
                assetHistoryItem.ShippingMethod = shippingMethod;
                assetHistoryItem.ReferenceNum = ReadString("ReferenceNumber");
                assetHistoryItem.Comments = ReadString("Comments");

                assetHistoryItem.DateCreated = now;
                assetHistoryItem.Actor = -2;
                assetHistoryItem.RefType = (int)ReferenceType.Contacts;
                assetHistoryItem.DateModified = now;
                assetHistoryItem.ModifierID = -2;

                assetHistory.Save();

                AssetAssignments assetAssignments = new AssetAssignments(_importUser);
                AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                assetAssignment.HistoryID = assetHistoryItem.HistoryID;

                assetAssignments.Save();

                string description = String.Format("Assigned asset to {0}.", contactAssignedTo[0].FirstLastName);
                ActionLogs.AddActionLog(_importUser, ActionLogType.Update, ReferenceType.Assets, asset.AssetID, description);
              }
              else if (contactAssignedTo.Count > 1)
              {
                // Log more than one email matching contact found
                continue;
              }
              else
              {
                // Log no email matching contact found
                continue;
              }
            }
            else
            {
              // Log no company or contact info to assign to
              continue;
            }
          }
        }

        if (isUpdate)
        {
          existingAsset.Save();
          // Add updated rows column as completed rows will reflect only adds
        }
        count++;

        if (count % BULK_LIMIT == 0)
        {
          assets.BulkSave();
          assets = new Assets(_importUser);
          UpdateImportCount(import, count);
        }
      }
      assets.BulkSave();
      UpdateImportCount(import, count);
      // _log.AppendMessage(count.ToString() + " Actions Imported.");
    }

    private void ImportCompanies(Import import)
    {
      SortedList<string, int> userList = GetUserAndContactList();

      Organizations companies = new Organizations(_importUser);
      int count = 0;
      while (_csv.ReadNextRecord())
      {
        string name = ReadString("Name");
        if (name == string.Empty)
        {
          //_log.AppendError(row, "Action skipped due to missing name");
          continue;
        }

        Organizations existingCompany = new Organizations(_importUser);
        Organization company = null;
        bool isUpdate = false;

        string importID = ReadString("ImportID");
        if (importID != string.Empty)
        {
          existingCompany.LoadByImportID(importID, _organizationID);
          if (existingCompany.Count == 1)
          {
            company = existingCompany[0];
            isUpdate = true;
          }
          else if (existingCompany.Count > 1)
          {
            // Log more than one company matching the importID was found.
            continue;
          }
        }

        if (company == null)
        {
          existingCompany = new Organizations(_importUser);
          existingCompany.LoadByName(name, _organizationID);
          if (existingCompany.Count > 0)
          {
            company = existingCompany[0];
            isUpdate = true;
          }
        }

        if (company == null)
        {
          company = companies.AddNewOrganization();
        }

        company.Name = name;
        company.Description = ReadString("Description");
        company.Website = ReadString("Website");
        company.CompanyDomains = ReadString("Domains");

        string emailOfDefaultSupportUser = ReadString("EmailOfDefaultSupportUser");
        if (!string.IsNullOrEmpty(emailOfDefaultSupportUser))
        {
          Users defaultSupportUser = new Users(_importUser);
          defaultSupportUser.LoadByEmail(emailOfDefaultSupportUser, _organizationID);
          if (defaultSupportUser.Count == 1)
          {
            company.DefaultSupportUserID = defaultSupportUser[0].UserID;
          }
          else if (defaultSupportUser.Count > 1)
          {
            // Log more than one user found matching email of default support user
          }
          else
          {
            // Log no user found matching email of default support user
          }
        }
          
        string defaultSupportGroupName = ReadString("DefaultSupportGroup");
        if (!string.IsNullOrEmpty(defaultSupportGroupName))
        {
          Groups defaultSupportGroup = new Groups(_importUser);
          defaultSupportGroup.LoadByGroupName(_organizationID, defaultSupportGroupName);
          if (defaultSupportGroup.Count == 1)
          {
            company.DefaultSupportGroupID = defaultSupportGroup[0].GroupID;
          }
          else if (defaultSupportGroup.Count > 1)
          {
            // Log more than one group found matching name of default support group
          }
          else
          {
            // Log no group found matching name of default support group
          }
        }
          
        //company.TimeZoneID = $("#ddlTz").val(); Not available in webapp
        company.SAExpirationDate = ReadDateNull("ServiceAgreementExpiration");

        string slaName = ReadString("ServiceLevelAgreement");
        if (!string.IsNullOrEmpty(slaName))
        {
          SlaLevels sla = new SlaLevels(_importUser);
          sla.LoadByName(_organizationID, slaName);
          if (sla.Count == 1)
          {
            company.SlaLevelID = sla[0].SlaLevelID;
          }
          else if (sla.Count > 1)
          {
            // Log more than one service level agreement found matching name.
          }
          else
          {
            // Log no service level agreement found matching name.
          }
        }
          
        company.SupportHoursMonth = ReadInt("SupportHoursPerMonth");
        company.IsActive = ReadBool("Active");
        company.HasPortalAccess = ReadBool("PortalAccess");
        company.IsApiEnabled = ReadBool("APIEnabled");
        company.IsApiActive = ReadBool("APIEnabled");
        company.InActiveReason = ReadString("InactiveReason");

        company.ExtraStorageUnits = 0;
        company.ImportID = ReadString("ImportID");
        company.IsCustomerFree = false;
        company.ParentID = _organizationID;
        company.PortalSeats = 0;
        company.PrimaryUserID = null;
        company.ProductType = ProductType.Express;
        company.UserSeats = 0;
        company.NeedsIndexing = true;
        if (!isUpdate)
        {
          company.SystemEmailID = Guid.NewGuid();
          company.WebServiceID = Guid.NewGuid();
        }
        DateTime? dateCreated = ReadDateNull("DateCreated");
        if (dateCreated != null)
        {
          company.DateCreated = (DateTime)dateCreated;
        }

        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID))
        {
          if (!userList.ContainsValue(creatorID))
          {
            creatorID = -2;
          }
        }
        company.CreatorID = creatorID;
        company.ModifierID = -2;

        if (isUpdate)
        {
          existingCompany.Save();
          // Add updated rows column as completed rows will reflect only adds
        }
        count++;

        if (count % BULK_LIMIT == 0)
        {
          companies.BulkSave();
          companies = new Organizations(_importUser);
          UpdateImportCount(import, count);
        }
      }
      companies.BulkSave();
      UpdateImportCount(import, count);
      // _log.AppendMessage(count.ToString() + " Actions Imported.");
    }

    private void ImportContacts(Import import)
    {
      SortedList<string, int> userList = GetUserAndContactList();

      Organization unknownCompany = Organizations.GetUnknownCompany(_loginUser, _organizationID);

      Users users = new Users(_importUser);
      int count = 0;
      while (_csv.ReadNextRecord())
      {
        string firstName = ReadString("FirstName");
        if (string.IsNullOrEmpty(firstName))
        {
          //_log.AppendError(row, "Action skipped due to missing first name");
          continue;
        }

        string lastName = ReadString("LastName");
        if (string.IsNullOrEmpty(lastName))
        {
          //_log.AppendError(row, "Action skipped due to missing last name");
          continue;
        }

        Users existingUser = new Users(_importUser);
        User user = null;
        bool isUpdate = false;
        int oldOrganizationID = 0;

        string importID = ReadString("ImportID");
        if (importID != string.Empty)
        {
          existingUser.LoadByImportID(importID, _organizationID);
          if (existingUser.Count == 1)
          {
            user = existingUser[0];
            oldOrganizationID = user.OrganizationID;
            isUpdate = true;
          }
          else if (existingUser.Count > 1)
          {
            // Log more than one user matching importID was found
            continue;
          }
        }

        if (user == null)
        {
          user = users.AddNewUser();
        }

        Organizations companies = new Organizations(_importUser);
        Organization company = null;

        string companyImportID = ReadString("CompanyImportID");
        if (!string.IsNullOrEmpty(companyImportID))
        {
          companies.LoadByImportID(companyImportID, _organizationID);
          if (companies.Count == 1)
          {
            company = companies[0];
            user.OrganizationID = company.OrganizationID;
          }
          else if (companies.Count > 1)
          {
            // log more than one company matching companyImportID found.
            continue;
          }
          else
          {
            // log no company matching companyImportID found.
            continue;
          }
        }

        if (company == null)
        {
          int? companyID = ReadIntNull("CompanyID");
          if (companyID != null)
          {
            companies = new Organizations(_importUser);
            companies.LoadByOrganizationID((int)companyID);
            if (companies.Count == 1 && companies[0].ParentID == _organizationID)
            {
              user.OrganizationID = companies[0].OrganizationID;
            }
            else if (companies[0].ParentID != _organizationID)
            {
              // log invalid companyID provided.
              continue;
            }
            else if (companies.Count > 1)
            {
              // log more than one company matching companyID found.
              continue;
            }
            else
            {
              // log no company matching companyID found.
              continue;
            }
          }
        }

        if (company == null)
        {
          string companyName = ReadString("CompanyName");
          if (!string.IsNullOrEmpty(companyName))
          {
            companies = new Organizations(_importUser);
            companies.LoadByOrganizationName(companyName, _organizationID);
            if (companies.Count == 1)
            {
              user.OrganizationID = companies[0].OrganizationID;
            }
            else if (companies.Count > 1)
            {
              // log more than one company matching CompanyName found.
              continue;
            }
            else
            {
              // log no company matching CompanyName found.
              continue;
            }
          }
        }

        if (company == null)
        {
          user.OrganizationID = unknownCompany.OrganizationID;
        }

        user.ImportID = ReadString("ImportID");
        user.FirstName = firstName;
        user.MiddleName = ReadString("MiddleName");
        user.LastName = lastName;
        user.Title = ReadString("Title");
        user.Email = ReadString("Email");
        user.BlockInboundEmail = ReadBool("PreventEmailFromCreatingAndUpdatingTickets");
        user.BlockEmailFromCreatingOnly = ReadBool("PreventEmailFromCreatingButAllowUpdatingTickets");
        user.IsPortalUser = ReadBool("PortalUser");

        string isActive = ReadString("IsActive");
        if (!string.IsNullOrEmpty(isActive))
        {
          user.IsActive = ReadBool("IsActive");
        }
        else
        {
          user.IsActive = true;
        }
        user.ActivatedOn = DateTime.UtcNow;
        user.CryptedPassword = "";
        user.DeactivatedOn = null;
        user.InOffice = false;
        user.InOfficeComment = "";
        user.IsFinanceAdmin = false;
        user.IsPasswordExpired = true;
        user.IsSystemAdmin = false;
        user.LastActivity = DateTime.UtcNow;
        user.LastLogin = DateTime.UtcNow;
        user.NeedsIndexing = true;
        user.PrimaryGroupID = null;
        DateTime? dateCreated = ReadDateNull("DateCreated");
        if (dateCreated != null)
        {
          user.DateCreated = (DateTime)dateCreated;
        }
        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID))
        {
          if (!userList.ContainsValue(creatorID))
          {
            creatorID = -2;
          }
        }
        user.CreatorID = creatorID;
        user.ModifierID = -2;

        if (isUpdate)
        {
          if (oldOrganizationID != user.OrganizationID)
          {
            Tickets t = new Tickets(_importUser);
            t.LoadByContact(user.UserID);

            foreach (Ticket tix in t)
            {
              tix.Collection.RemoveContact(user.UserID, tix.TicketID);
            }

            existingUser.Save();

            foreach (Ticket tix in t)
            {
              tix.Collection.AddContact(user.UserID, tix.TicketID);

            }

            EmailPosts ep = new EmailPosts(_importUser);
            ep.LoadByRecentUserID(user.UserID);
            ep.DeleteAll();
            ep.Save();

            //Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), value);
            //string description = String.Format("{0} set contact company to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, organization.Name);
            //ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Users, userID, description);

          }
          else
          {
            existingUser.Save();
          }
          // Add updated rows column as completed rows will reflect only adds
        }
        count++;

        if (count % BULK_LIMIT == 0)
        {
          users.BulkSave();
          users = new Users(_importUser);
          UpdateImportCount(import, count);
        }
      }
      users.BulkSave();
      UpdateImportCount(import, count);
    }

    private void ImportTickets(Import import)
    {
      TicketTypes ticketTypes = new TicketTypes(_importUser);
      ticketTypes.LoadAllPositions(_organizationID);

      TicketStatuses ticketStatuses = new TicketStatuses(_importUser);
      ticketStatuses.LoadByOrganizationID(_organizationID);

      TicketSeverities ticketSeverities = new TicketSeverities(_importUser);
      ticketSeverities.LoadByOrganizationID(_organizationID);

      Tickets users = new Tickets(_importUser);
      int count = 0;
      while (_csv.ReadNextRecord())
      {
        string name = ReadString("Name");
        if (string.IsNullOrEmpty(name))
        {
          //_log.AppendError(row, "Ticket skipped due to missing name");
          continue;
        }

        TicketType ticketType = null;
        string ticketTypeString = ReadString("Type");
        if (string.IsNullOrEmpty(ticketTypeString))
        {
          //_log.AppendError(row, "Ticket skipped due to missing ticket type");
          continue;
        }
        else
        {
          ticketType = ticketTypes.FindByName(ticketTypeString);
          if (ticketType == null)
          {
            ticketType = ticketTypes.AddNewTicketType();
            ticketType.Name = ticketTypeString;
            ticketType.Description = ticketTypeString;
            ticketType.Position = ticketTypes.GetMaxPosition(_organizationID) + 1;
            ticketType.OrganizationID = _organizationID;
            ticketTypes.Save();
            ticketTypes.ValidatePositions(_organizationID);

            TicketStatuses newTicketStatuses = new TicketStatuses(_importUser);
            TicketStatus newTicketStatus = ticketStatuses.AddNewTicketStatus();
            newTicketStatus.Name = "New";
            newTicketStatus.Description = "New";
            newTicketStatus.Position = 0;
            newTicketStatus.OrganizationID = _organizationID;
            newTicketStatus.TicketTypeID = ticketType.TicketTypeID;
            newTicketStatus.IsClosed = false;
            newTicketStatus.IsClosedEmail = false;

            newTicketStatus = ticketStatuses.AddNewTicketStatus();
            newTicketStatus.Name = "Closed";
            newTicketStatus.Description = "Closed";
            newTicketStatus.Position = 30;
            newTicketStatus.OrganizationID = _organizationID;
            newTicketStatus.TicketTypeID = ticketType.TicketTypeID;
            newTicketStatus.IsClosed = true;
            newTicketStatus.IsClosedEmail = false;
            newTicketStatus.Collection.Save();
            newTicketStatus.Collection.ValidatePositions(_organizationID);
          }
        }

        TicketStatus ticketStatus = null;
        string ticketStatusString = ReadString("Status");
        if (string.IsNullOrEmpty(ticketStatusString))
        {
          //_log.AppendError(row, "Ticket skipped due to missing ticket type");
          continue;
        }
        else
        {
          ticketStatus = ticketStatuses.FindByName(ticketStatusString, ticketType.TicketTypeID);
          if (ticketStatus == null)
          {
            ticketStatus = ticketStatuses.AddNewTicketStatus();
            ticketStatus.Name = ticketTypeString;
            ticketStatus.Description = ticketTypeString;
            ticketStatus.Position = ticketStatuses.GetMaxPosition(ticketType.TicketTypeID) + 1;
            ticketStatus.OrganizationID = _organizationID;
            ticketStatus.TicketTypeID = ticketType.TicketTypeID;
            ticketStatus.IsClosed = false;
            ticketStatus.IsClosedEmail = false;
            ticketStatuses.Save();
            ticketStatuses.ValidatePositions(_organizationID);
          }
        }

        TicketSeverity ticketSeverity = null;
        string ticketSeverityString = ReadString("Severity");
        if (string.IsNullOrEmpty(ticketSeverityString))
        {
          //_log.AppendError(row, "Ticket skipped due to missing ticket severity");
          continue;
        }
        else
        {
          ticketStatus = ticketStatuses.FindByName(ticketStatusString, ticketType.TicketTypeID);
          if (ticketStatus == null)
          {
            ticketStatus = ticketStatuses.AddNewTicketStatus();
            ticketStatus.Name = ticketTypeString;
            ticketStatus.Description = ticketTypeString;
            ticketStatus.Position = ticketStatuses.GetMaxPosition(ticketType.TicketTypeID) + 1;
            ticketStatus.OrganizationID = _organizationID;
            ticketStatus.TicketTypeID = ticketType.TicketTypeID;
            ticketStatus.IsClosed = false;
            ticketStatus.IsClosedEmail = false;
            ticketStatuses.Save();
            ticketStatuses.ValidatePositions(_organizationID);
          }
        }

      }
    }

    private void ImportCustomFieldPickList(Import import)
    {
      int count = 0;
      var fields = new Dictionary<string, List<string>>();
      
      while (_csv.ReadNextRecord())
      {
        count++;
        string apiFieldName = ReadString("ApiFieldName");
        string listValue = ReadString("PickListValue");
        List<string> list;
        if (!fields.TryGetValue(apiFieldName, out list))
        {
          list = new List<string>();
          fields.Add(apiFieldName, list);
        }

        list.Add(listValue);
      }

      foreach (KeyValuePair<string, List<string>> item in fields)
	    {
        CustomField customField = CustomFields.GetCustomFieldByApi(_importUser, _organizationID, item.Key);
        if (customField != null)
        {
          customField.ListValues = string.Join("|", item.Value.ToArray());
          customField.Collection.Save();
        }
	    }

      UpdateImportCount(import, count);
    }
    
    private void ClearEmails()
    {
      try
      {
        EmailPosts.DeleteImportEmails(_importUser);

      }
      catch (Exception)
      {
        
      }
    }

    private string GetMappedName(string field)
    {
      //return _map[field];
      return field;
    }

    private string GetMappedValue(string field)
    {
      string mappedField = GetMappedName(field);
      return _headers.Contains(mappedField) ? _csv[mappedField] : "";
    }
    
    private DateTime ReadDate(string field, DateTime defaultValue)
    {
      string value = GetMappedValue(field);
      DateTime result = defaultValue;
      DateTime.TryParse(value, out result);
      return result;
    }

    private DateTime? ReadDateNull(string field)
    {
      string value = GetMappedValue(field);
      DateTime result;
      if (!DateTime.TryParse(value, out result))
      {
        return null;
      }
      return result;
    }

    private bool ReadBool(string field)
    {
      string value = GetMappedValue(field);
      value = value.ToLower();
      return value.IndexOf('t') > -1 || value.IndexOf('y') > -1 || value.IndexOf('1') > -1;
    }

    private int ReadInt(string field, int defaultValue = 0)
    {
      string value = GetMappedValue(field);
      int result = defaultValue;
      int.TryParse(value, out result);
      return result;
    }

    private int? ReadIntNull(string field)
    {
      string value = GetMappedValue(field);
      int result;
      if (!int.TryParse(value, out result))
      {
        return null;
      }
      return result;
    }

    private string ReadString(string field)
    {
      return GetMappedValue(field).Trim();
    }

    private SystemActionType GetSystemActionTypeID(string name)
    {
      SystemActionType result = SystemActionType.Custom;
      switch (name.ToLower().Trim())
      {
        case "description": result = SystemActionType.Description; break;
        case "resolution": result = SystemActionType.Resolution; break;
        case "updaterequest": result = SystemActionType.UpdateRequest; break;
        case "email": result = SystemActionType.Email; break;
        case "reminder": result = SystemActionType.Reminder; break;
        default:
          break;
      }
      return result;
    }

    private int? GetActionTypeID(ActionTypes actionTypes, string name)
    {
      name = name.Trim();
      if (GetSystemActionTypeID(name) != SystemActionType.Custom) return null;
      if (string.IsNullOrWhiteSpace(name)) name = "Comment";
      ActionType actionType = actionTypes.FindByName(name);

      if (actionType == null)
      {
        ActionTypes ats = new ActionTypes(_importUser);
        actionType = ats.AddNewActionType();
        actionType.Name = name;
        actionType.Description = "";
        actionType.IsTimed = true;
        actionType.OrganizationID = _organizationID;
        actionType.Position = actionTypes.GetMaxPosition(_organizationID) + 1;
        ats.Save();
        int? result = actionType.ActionTypeID;
        actionTypes.LoadAllPositions(_organizationID);
        return result;
      }
      else
      {
        return actionType.ActionTypeID;
      }
    }

    private string ConvertHtmlLineBreaks(string text, string lineBreak = "<br />")
    {
      return Regex.Replace(text, @"\r\n?|\n", lineBreak);
    }

    private SortedList<string, int> GetTicketList()
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"SELECT TicketNumber, TicketID
                              FROM Tickets
                              WHERE (OrganizationID = @OrganizationID)";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return GetList(command);
    }

    private SortedList<string, int> GetUserAndContactList()
    {
      SortedList<string, int> list = GetUserList();
      return GetContactList(list);
    }

    private SortedList<string, int> GetUserList()
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
SELECT DISTINCT REPLACE(u.Email, ' ', ''), MAX(u.UserID) AS UserID
FROM Users u 
WHERE (u.OrganizationID = @OrganizationID)
AND (u.MarkDeleted = 0)
GROUP BY REPLACE(u.Email, ' ', '')
";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return GetList(command);
    }

    private SortedList<string, int> GetContactList(SortedList<string,int> list = null)
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
SELECT DISTINCT(REPLACE(u.Email + '(' + o.Name + ')', ' ', '')), MAX(u.UserID)
FROM Users u 
LEFT JOIN Organizations o
ON o.OrganizationID = u.OrganizationID
WHERE (o.ParentID = @OrganizationID)
AND (u.MarkDeleted = 0)
GROUP BY REPLACE(u.Email + '(' + o.Name + ')', ' ', '')";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return list == null ? GetList(command) : GetList(command, list);
    }

    private SortedList<string, int> GetList(SqlCommand command)
    {
      return GetList(command, new SortedList<string, int>());
    }


    private SortedList<string, int> GetList(SqlCommand command, SortedList<string, int> list)
    {
   
      using (SqlConnection connection = new SqlConnection(_importUser.ConnectionString))
      {
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        command.Connection = connection;
        command.Transaction = transaction;
        SqlDataReader reader = command.ExecuteReader();
        try
        {
          while (reader.Read())
          {
            list.Add((reader[0].ToString()).Trim().ToUpper(), reader[1] as int? ?? -1);
          }
        }
        finally
        {
          reader.Close();
        }

        transaction.Commit();
      }
      return list;
    
    }
  }
}
