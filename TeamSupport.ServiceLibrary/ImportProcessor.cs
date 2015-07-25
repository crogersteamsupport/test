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
    private ImportFieldsView _map;
    private string[] _headers;
    private ImportLog _importLog;

    public override void Run()
    {
      Imports imports = new Imports(LoginUser);
      imports.LoadWaiting();

      try
      {
        if (!imports.IsEmpty)
        {
          string logPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Logs");
          logPath = Path.Combine(logPath, imports[0].OrganizationID.ToString());
          _importLog = new ImportLog(logPath, imports[0].ImportID);
          _importLog.Write("Importing importID: " + imports[0].ImportID.ToString());

          ProcessImport(imports[0]);
        }
        UpdateHealth();
      }
      catch (Exception ex)
      {
        _importLog.Write(ex.Message);
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

      try
      {
        import.TotalRows = GetTotalRows(csvFile);
      }
      catch (Exception ex)
      {
        _importLog.Write(ex.Message);
      }
      import.CompletedRows = 0;
      import.IsRunning = true;
      import.DateStarted = DateTime.UtcNow;
      import.Collection.Save();

      _map = new ImportFieldsView(_importUser);
      _map.LoadByImportID(import.ImportID);

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
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportCustomFields(import.RefType);
            break;
          case ReferenceType.Organizations:
            ImportCompanies(import);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportCustomFields(import.RefType);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportAddresses(import, ReferenceType.Organizations);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportPhoneNumbers(import, ReferenceType.Organizations);
            break;
          case ReferenceType.CompanyAddresses:
            ImportAddresses(import, ReferenceType.Organizations);
            break;
          case ReferenceType.CompanyPhoneNumbers:
            ImportPhoneNumbers(import, ReferenceType.Organizations);
            break;
          case ReferenceType.Contacts:
            ImportCompanies(import);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportContacts(import);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportCustomFields(import.RefType);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportAddresses(import, ReferenceType.Contacts);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportPhoneNumbers(import, ReferenceType.Contacts);
            break;
          case ReferenceType.ContactAddresses:
            ImportAddresses(import, ReferenceType.Contacts);
            break;
          case ReferenceType.ContactPhoneNumbers:
            ImportPhoneNumbers(import, ReferenceType.Contacts);
            break;
          case ReferenceType.Tickets:
            ImportTickets(import);
            _csv = new CsvReader(new StreamReader(csvFile), true);
            ImportCustomFields(import.RefType);
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
        Ticket ticket = null;
        string ticketImportID = ReadString("TicketImportID");
        if (!string.IsNullOrEmpty(ticketImportID))
        {
          Tickets tickets = new Tickets(_importUser);
          tickets.LoadByImportID(ticketImportID, _organizationID);
          if (tickets.Count == 1)
          {
            ticket = tickets[0];
          }
          else if (tickets.Count > 1)
          {
            _importLog.Write("More than one ticket found matching TicketImportID");
            continue;
          }
        }

        int ticketID = ReadInt("TicketID");
        if (!ticketList.ContainsValue(ticketID) && ticket == null)
        {
          _importLog.Write("Action skipped due to missing ticket");
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
              _importLog.Write("More than one action matching the importID was found");
              continue;
            }
          }
        //}

        if (action == null)
        {
          action = actions.AddNewAction();
        }

        string actionType = ReadString("Type");
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
        action.IsVisibleOnPortal = ReadBool("Visible");
        action.ModifierID = -2;
        action.Name = "";
        action.TicketID = ticketID;
        action.ImportID = ReadString("ImportID");
        action.TimeSpent = ReadIntNull("TimeSpent");

        action.Pinned = ReadBool("IsPinned");

        if (isUpdate)
        {
          existingAction.Save();
          _importLog.Write("ActionID " + action.ActionID.ToString() + " was updated.");
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
      _importLog.Write(count.ToString() + " actions imported.");
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
          _importLog.Write("No product found.");
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
              _importLog.Write("More than one asset already exists in the database with given importID");
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
            _importLog.Write("No product version found.");
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
                    _importLog.Write("DateShipped and shippingMethod are required.");
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
                  _importLog.Write("More than one email matching contact found.");
                  continue;
                }
                else
                {
                  _importLog.Write("No email matching contact found.");
                  continue;
                }
              }
              else
              {
                DateTime? dateShipped = ReadDateNull("DateShipped");
                string shippingMethod = ReadString("ShippingMethod");
                if (dateShipped == null || shippingMethod == string.Empty)
                {
                  _importLog.Write("DateShipped and shippingMethod are required.");
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
              _importLog.Write("More than one name matching company found.");
              continue;
            }
            else
            {
              _importLog.Write("No name matching company found.");
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
                  _importLog.Write("DateShipped and shippingMethod are required");
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
                _importLog.Write("More than one email matching contact found.");
                continue;
              }
              else
              {
                _importLog.Write("No email matching contact found.");
                continue;
              }
            }
            else
            {
              _importLog.Write("No company or contact info to assign to.");
              continue;
            }
          }
        }

        if (isUpdate)
        {
          existingAsset.Save();
          _importLog.Write("AssetID " + asset.AssetID.ToString() + " was updated.");
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
      _importLog.Write(count.ToString() + " assets imported.");
    }

    private void ImportCustomFields(ReferenceType refType)
    {
      SortedList<string, int> assetList = null;
      SortedList<string, int> contactList = null;
      SortedList<string, int> ticketList = null;

      switch (refType)
      {
        case ReferenceType.Assets:
          assetList = GetAssetList();
          break;
        case ReferenceType.Contacts:
          contactList = GetContactList();
          break;
        case ReferenceType.Tickets:
          ticketList = GetTicketList();
          break;
      }

      SortedList<string, int> userList = GetUserAndContactList();
      CustomValues customValues = new CustomValues(_importUser);
      int count = 0;
      while (_csv.ReadNextRecord())
      {
        int refID = 0;
        string errorMessage = string.Empty;
        switch (refType)
        {
          case ReferenceType.Assets:
            string assetName = ReadString("Name");
            string assetSerialNumber = ReadString("SerialNumber");
            string location = ReadString("Location");
            switch (location.Trim().ToLower())
            {
              case "assigned":
                location = "1";
                break;
              case "warehouse":
                location = "2";
                break;
              case "junkyard":
                location = "3";
                break;
              default:
                location = "2";
                break;
            }
            string key = assetSerialNumber + assetName + location;
            if (!assetList.TryGetValue(key.ToUpper().Replace(" ", string.Empty), out refID))
            {
              errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as asset " + assetName + " does not exists.";
            }
            break;
          case ReferenceType.Organizations:
            string companyName = ReadString("CompanyName");
            Organizations companiesMatchingName = new Organizations(_importUser);
            companiesMatchingName.LoadByName(companyName, _organizationID);
            if (companiesMatchingName.Count > 0)
            {
              refID = companiesMatchingName[0].OrganizationID;
            }
            else
            {
              errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as company " + companyName + " does not exists.";
            }
            break;
          case ReferenceType.Contacts:
            companyName = string.Empty;
            companyName = ReadString("CompanyName");
            string contactEmail = ReadString("ContactEmail");
            key = string.Empty;
            key = contactEmail + "(" + companyName + ")";
            if (!contactList.TryGetValue(key.ToUpper().Replace(" ", string.Empty), out refID))
            {
              errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as contact " + key + " does not exists.";
            }
            break;
          case ReferenceType.Tickets:
            string ticketNumber = ReadString("TicketNumber");
            if (!ticketList.TryGetValue(ticketNumber, out refID))
            {
              errorMessage = "Custom fields in row index " + _csv.CurrentRecordIndex + "could not be imported as ticket# " + ticketNumber + " does not exists.";
            }
            break;
        }

        if (errorMessage != string.Empty)
        {
          _importLog.Write(errorMessage);
          continue;
        }

        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID))
        {
          if (!userList.ContainsValue(creatorID))
          {
            creatorID = -2;
          }
        }
        DateTime? dateCreated = ReadDateNull("DateCreated");

        ImportFieldsView fields = new ImportFieldsView(_importUser);
        fields.LoadByRefType((int)refType);
        foreach (ImportFieldsViewItem field in fields)
        {
          if (field.IsCustom != null && (bool)field.IsCustom)
          {
            string value = ReadString(field.FieldName);
            if (!string.IsNullOrEmpty(value.Trim()))
            {
              CustomValues existingCustomValue = new CustomValues(_importUser);
              existingCustomValue.LoadByFieldID(field.ImportFieldID, refID);
              if (existingCustomValue.Count > 0)
              {
                existingCustomValue[0].Value = value;
                existingCustomValue[0].ModifierID = -2;
                existingCustomValue.Save();
              }
              else
              {
                CustomValue customValue = customValues.AddNewCustomValue();
                customValue.RefID = refID;
                customValue.Value = value;
                customValue.CustomFieldID = field.ImportFieldID;
                if (dateCreated != null)
                {
                  customValue.DateCreated = (DateTime)dateCreated;
                }
                customValue.CreatorID = creatorID;
                customValue.ModifierID = -2;
                count++;

                if (count % BULK_LIMIT == 0)
                {
                  customValues.BulkSave();
                  count = 0;
                  customValues = new CustomValues(_importUser);
                }
              }
            }
          }
        }
      }
      customValues.BulkSave();
    }

    private void ImportCompanies(Import import)
    {
      SortedList<string, int> userList = GetUserAndContactList();

      Organizations companies = new Organizations(_importUser);

      int count = 0;
      while (_csv.ReadNextRecord())
      {
        string name = ReadString("CompanyName");
        if (name == string.Empty)
        {
          _importLog.Write("Action skipped due to missing name");
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
            _importLog.Write("More than one company matching the importID was found.");
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
            _importLog.Write("More than one user found matching email of default support user.");
          }
          else
          {
            _importLog.Write("No user found matching email of default support user.");
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
            _importLog.Write("More than one group found matching name of default support group.");
          }
          else
          {
            _importLog.Write("No group found matching name of default support group.");
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
            _importLog.Write("More than one service level agreement found matching name.");
          }
          else
          {
            _importLog.Write("No service level agreement found matching name.");
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
          _importLog.Write("CompanyID " + company.OrganizationID.ToString() + " was updated.");
        }
        else
        {
          _importLog.Write("Company " + company.Name + " was added to import set.");
        }
        count++;

        if (count % BULK_LIMIT == 0)
        {
          companies.BulkSave();
          companies = new Organizations(_importUser);
          UpdateImportCount(import, count);
          _importLog.Write("Import set with " + count.ToString() + " companies inserted in database.");
        }
      }
      companies.BulkSave();
      UpdateImportCount(import, count);
      _importLog.Write("Import set with " + count.ToString() + " companies inserted in database.");
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
          _importLog.Write("Contact skipped due to missing first name");
          continue;
        }

        string lastName = ReadString("LastName");
        if (string.IsNullOrEmpty(lastName))
        {
          _importLog.Write("Contact skipped due to missing last name");
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
            _importLog.Write("More than one user matching importID was found");
            continue;
          }
        }

        string email = ReadString("ContactEmail");
        if (email != string.Empty)
        {
          existingUser = new Users(_importUser);
          existingUser.LoadByEmail(_organizationID, email);
          if (existingUser.Count > 0)
          {
            user = existingUser[0];
            oldOrganizationID = user.OrganizationID;
            isUpdate = true;
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
            _importLog.Write("More than one company matching companyImportID found.");
            continue;
          }
          else
          {
            _importLog.Write("No company matching companyImportID found.");
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
              company = companies[0];
              user.OrganizationID = companies[0].OrganizationID;
            }
            else if (companies[0].ParentID != _organizationID)
            {
              _importLog.Write("Invalid companyID provided.");
              continue;
            }
            else if (companies.Count > 1)
            {
              _importLog.Write("More than one company matching companyID found.");
              continue;
            }
            else
            {
              _importLog.Write("No company matching companyID found.");
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
              company = companies[0];
              user.OrganizationID = companies[0].OrganizationID;
            }
            else if (companies.Count > 1)
            {
              _importLog.Write("More than one company matching CompanyName found.");
              continue;
            }
            else
            {
              _importLog.Write("No company matching CompanyName found.");
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
        user.Email = ReadString("ContactEmail");
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
            _importLog.Write("UserID " + user.UserID.ToString() + " was updated.");
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
      _importLog.Write(count.ToString() + " contacts imported.");
    }

    private void ImportAddresses(Import import, ReferenceType addressReferenceType)
    {
      SortedList<string, int> userList = GetUserList();
      SortedList<string, int> contactList = null;
      if (addressReferenceType == ReferenceType.Contacts)
      {
        contactList = GetContactList();
      }

      Addresses addresses = new Addresses(_importUser);
      int count = 0;
      while (_csv.ReadNextRecord())
      {
        long index = _csv.CurrentRecordIndex + 1;
        DateTime now = DateTime.UtcNow;
        Addresses newAddresses = new Addresses(_importUser);
        Address newAddress = newAddresses.AddNewAddress();
        newAddress.RefType = addressReferenceType;

        DateTime? dateCreated = ReadDateNull("DateCreated");
        if (dateCreated != null)
        {
          newAddress.DateCreated = (DateTime)dateCreated;
        }

        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID))
        {
          if (!userList.ContainsValue(creatorID))
          {
            creatorID = -2;
          }
        }
        newAddress.CreatorID = creatorID;
        newAddress.ModifierID = -2;

        string companyName = ReadString("CompanyName");
        int orgID;
        Organizations organizationsMatchingName = new Organizations(_importUser);
        organizationsMatchingName.LoadByName(companyName, _organizationID);
        if (organizationsMatchingName.Count == 0)
        {
          Organizations newCompanies = new Organizations(_importUser);
          Organization newCompany = newCompanies.AddNewOrganization();
          newCompany.Name = companyName;
          newCompany.ParentID = _organizationID;
          newCompany.IsActive = true;
          newCompany.ExtraStorageUnits = 0;
          newCompany.IsCustomerFree = false;
          newCompany.PortalSeats = 0;
          newCompany.PrimaryUserID = null;
          newCompany.ProductType = ProductType.Express;
          newCompany.UserSeats = 0;
          newCompany.NeedsIndexing = true;
          newCompany.SystemEmailID = Guid.NewGuid();
          newCompany.WebServiceID = Guid.NewGuid();
          if (dateCreated != null)
          {
            newCompany.DateCreated = (DateTime)dateCreated;
          }
          newCompany.CreatorID = creatorID;
          newCompany.ModifierID = -2;
          newCompanies.Save();
          orgID = newCompany.OrganizationID;
        }
        else
        {
          orgID = organizationsMatchingName[0].OrganizationID;
        }

        switch (addressReferenceType)
        {
          case ReferenceType.Organizations:
            newAddress.RefID = orgID;
            break;
          case ReferenceType.Contacts:
            string contactEmail = ReadString("ContactEmail");
            int contactID;
            string searchTerm = contactEmail.Replace(" ", string.Empty) + "(" + companyName.Replace(" ", string.Empty) + ")";
            if (!contactList.TryGetValue(searchTerm.ToUpper(), out contactID))
            {
              string firstName = ReadString("FirstName");
              string lastName = ReadString("LastName");
              if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
              {
                Users newContacts = new Users(_importUser);
                User newContact = newContacts.AddNewUser();
                newContact.FirstName = firstName;
                newContact.LastName = lastName;
                newContact.Email = contactEmail;
                newContact.OrganizationID = orgID;
                newContact.ActivatedOn = now;
                newContact.CryptedPassword = "";
                newContact.DeactivatedOn = null;
                newContact.InOffice = false;
                newContact.InOfficeComment = "";
                newContact.IsFinanceAdmin = false;
                newContact.IsPasswordExpired = true;
                newContact.IsSystemAdmin = false;
                newContact.LastActivity = now;
                newContact.LastLogin = now;
                newContact.NeedsIndexing = true;
                newContact.PrimaryGroupID = null;
                if (dateCreated != null)
                {
                  newContact.DateCreated = (DateTime)dateCreated;
                }
                newContact.CreatorID = creatorID;
                newContact.ModifierID = -2;
                newContacts.Save();
                contactID = newContact.UserID;
                contactList.Add(searchTerm, contactID);
              }
              else
              {
                _importLog.Write("Address in row " + index.ToString() + " could not be added as contact does not exists and either first or last name are missing.");
                continue;
              }
            }
            newAddress.RefID = contactID;
            break;
        }

        newAddress.Description = ReadString("AddressDescription");
        newAddress.Addr1 = ReadString("Addr1");
        newAddress.Addr2 = ReadString("Addr2");
        newAddress.Addr3 = ReadString("Addr3");
        newAddress.City = ReadString("City");
        newAddress.State = ReadString("State");
        newAddress.Zip = ReadString("Zip");
        newAddress.Country = ReadString("Country");
        newAddress.Comment = ReadString("AddressComment");

        Addresses existingAddresses = new Addresses(_importUser);
        existingAddresses.LoadByID(newAddress.RefID, addressReferenceType);
        bool alreadyExists = false;
        foreach (Address existingAddress in existingAddresses)
        {
          if (
            newAddress.Description.Replace(" ", string.Empty).ToLower() == existingAddress.Description.Replace(" ", string.Empty).ToLower() 
            && newAddress.Addr1.Replace(" ", string.Empty).ToLower() == existingAddress.Addr1.Replace(" ", string.Empty).ToLower()
            && newAddress.Addr2.Replace(" ", string.Empty).ToLower() == existingAddress.Addr2.Replace(" ", string.Empty).ToLower()
            && newAddress.Addr3.Replace(" ", string.Empty).ToLower() == existingAddress.Addr3.Replace(" ", string.Empty).ToLower()
            && newAddress.City.Replace(" ", string.Empty).ToLower() == existingAddress.City.Replace(" ", string.Empty).ToLower()
            && newAddress.State.Replace(" ", string.Empty).ToLower() == existingAddress.State.Replace(" ", string.Empty).ToLower()
            && newAddress.Zip.Replace(" ", string.Empty).ToLower() == existingAddress.Zip.Replace(" ", string.Empty).ToLower()
            && newAddress.Country.Replace(" ", string.Empty).ToLower() == existingAddress.Country.Replace(" ", string.Empty).ToLower()
            && newAddress.Comment.Replace(" ", string.Empty).ToLower() == existingAddress.Comment.Replace(" ", string.Empty).ToLower())
          {
            alreadyExists = true;
            break;
          }
        }

        if (!alreadyExists)
        {
          Address address = addresses.AddNewAddress();
          address.RefType = newAddress.RefType;
          address.DateCreated = newAddress.DateCreated;
          address.CreatorID = newAddress.CreatorID;
          address.ModifierID = newAddress.ModifierID;
          address.RefID = newAddress.RefID;
          address.Description = newAddress.Description;
          address.Addr1 = newAddress.Addr1;
          address.Addr2 = newAddress.Addr2;
          address.Addr3 = newAddress.Addr3;
          address.City = newAddress.City;
          address.State = newAddress.State;
          address.Zip = newAddress.Zip;
          address.Country = newAddress.Country;
          address.Comment = newAddress.Comment;
          _importLog.Write("Address in row " + index.ToString() + " was added to addresses set.");
        }
        else
        {
          _importLog.Write("Address in row " + index.ToString() + " already exists and was not added to addresses set.");
        }
        count++;

        if (count % BULK_LIMIT == 0)
        {
          addresses.BulkSave();
          addresses = new Addresses(_importUser);
          UpdateImportCount(import, count);
          _importLog.Write("Import set with " + count.ToString() + " addresses inserted in database.");
        }
      }
      addresses.BulkSave();
      UpdateImportCount(import, count);
      _importLog.Write("Import set with " + count.ToString() + " addresses inserted in database.");
    }

    private void ImportPhoneNumbers(Import import, ReferenceType phoneNumberReferenceType)
    {
      SortedList<string, int> userList = GetUserList();
      SortedList<string, int> contactList = null;
      if (phoneNumberReferenceType == ReferenceType.Contacts)
      {
        contactList = GetContactList();
      }

      PhoneTypes phoneTypes = new PhoneTypes(_importUser);
      phoneTypes.LoadByOrganizationID(_organizationID);

      PhoneNumbers phoneNumbers = new PhoneNumbers(_importUser);
      int count = 0;
      int bulkCount = 0;
      while (_csv.ReadNextRecord())
      {
        long index = _csv.CurrentRecordIndex + 1;
        DateTime now = DateTime.UtcNow;
        PhoneNumbers newPhoneNumbers = new PhoneNumbers(_importUser);
        PhoneNumber newPhoneNumber = newPhoneNumbers.AddNewPhoneNumber();
        PhoneNumber newPhoneNumber2 = newPhoneNumbers.AddNewPhoneNumber();
        PhoneNumber newPhoneNumber3 = newPhoneNumbers.AddNewPhoneNumber();
        newPhoneNumber.RefType = phoneNumberReferenceType;
        newPhoneNumber2.RefType = phoneNumberReferenceType;
        newPhoneNumber3.RefType = phoneNumberReferenceType;

        DateTime? dateCreated = ReadDateNull("DateCreated");
        if (dateCreated != null)
        {
          newPhoneNumber.DateCreated = (DateTime)dateCreated;
          newPhoneNumber2.DateCreated = (DateTime)dateCreated;
          newPhoneNumber3.DateCreated = (DateTime)dateCreated;
        }

        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID))
        {
          if (!userList.ContainsValue(creatorID))
          {
            creatorID = -2;
          }
        }
        newPhoneNumber.CreatorID = creatorID;
        newPhoneNumber2.CreatorID = creatorID;
        newPhoneNumber3.CreatorID = creatorID;
        
        newPhoneNumber.ModifierID = -2;
        newPhoneNumber2.ModifierID = -2;
        newPhoneNumber3.ModifierID = -2;

        string companyName = ReadString("CompanyName");
        int orgID;
        Organizations organizationsMatchingName = new Organizations(_importUser);
        organizationsMatchingName.LoadByName(companyName, _organizationID);
        if (organizationsMatchingName.Count == 0)
        {
          Organizations newCompanies = new Organizations(_importUser);
          Organization newCompany = newCompanies.AddNewOrganization();
          newCompany.Name = companyName;
          newCompany.ParentID = _organizationID;
          newCompany.IsActive = true;
          newCompany.ExtraStorageUnits = 0;
          newCompany.IsCustomerFree = false;
          newCompany.PortalSeats = 0;
          newCompany.PrimaryUserID = null;
          newCompany.ProductType = ProductType.Express;
          newCompany.UserSeats = 0;
          newCompany.NeedsIndexing = true;
          newCompany.SystemEmailID = Guid.NewGuid();
          newCompany.WebServiceID = Guid.NewGuid();
          if (dateCreated != null)
          {
            newCompany.DateCreated = (DateTime)dateCreated;
          }
          newCompany.CreatorID = creatorID;
          newCompany.ModifierID = -2;
          newCompanies.Save();
          orgID = newCompany.OrganizationID;
        }
        else
        {
          orgID = organizationsMatchingName[0].OrganizationID;
        }

        switch (phoneNumberReferenceType)
        {
          case ReferenceType.Organizations:
            newPhoneNumber.RefID = orgID;
            newPhoneNumber2.RefID = orgID;
            newPhoneNumber3.RefID = orgID;
            break;
          case ReferenceType.Contacts:
            string contactEmail = ReadString("ContactEmail");
            int contactID;
            string searchTerm = contactEmail.Replace(" ", string.Empty) + "(" + companyName.Replace(" ", string.Empty) + ")";
            if (!contactList.TryGetValue(searchTerm.ToUpper(), out contactID))
            {
              string firstName = ReadString("FirstName");
              string lastName = ReadString("LastName");
              if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
              {
                Users newContacts = new Users(_importUser);
                User newContact = newContacts.AddNewUser();
                newContact.FirstName = firstName;
                newContact.LastName = lastName;
                newContact.Email = contactEmail;
                newContact.OrganizationID = orgID;
                newContact.ActivatedOn = now;
                newContact.CryptedPassword = "";
                newContact.DeactivatedOn = null;
                newContact.InOffice = false;
                newContact.InOfficeComment = "";
                newContact.IsFinanceAdmin = false;
                newContact.IsPasswordExpired = true;
                newContact.IsSystemAdmin = false;
                newContact.LastActivity = now;
                newContact.LastLogin = now;
                newContact.NeedsIndexing = true;
                newContact.PrimaryGroupID = null;
                if (dateCreated != null)
                {
                  newContact.DateCreated = (DateTime)dateCreated;
                }
                newContact.CreatorID = creatorID;
                newContact.ModifierID = -2;
                newContacts.Save();
                contactID = newContact.UserID;
                contactList.Add(searchTerm, contactID);
              }
              else
              {
                _importLog.Write("Phone number in row " + index.ToString() + " could not be added as contact does not exists and either first or last name are missing.");
                continue;
              }
            }
            newPhoneNumber.RefID = contactID;
            newPhoneNumber2.RefID = contactID;
            newPhoneNumber3.RefID = contactID;
            break;
        }

        string phoneTypeName = ReadString("PhoneType");
        string phoneTypeName2 = ReadString("PhoneType2");
        string phoneTypeName3 = ReadString("PhoneType3");
        PhoneType phoneType = phoneTypes.FindByName(phoneTypeName);
        int phoneTypeID = 0;
        if (phoneType == null)
        {
          PhoneTypes newPhoneTypes = new PhoneTypes(_importUser);
          PhoneType newPhoneType = newPhoneTypes.AddNewPhoneType();
          newPhoneType.Name = phoneTypeName;
          newPhoneType.OrganizationID = _organizationID;
          if (dateCreated != null)
          {
            newPhoneType.DateCreated = (DateTime)dateCreated;
          }
          newPhoneType.CreatorID = creatorID;
          newPhoneTypes.Save();
          phoneTypeID = newPhoneType.PhoneTypeID;

          phoneTypes = new PhoneTypes(_importUser);
          phoneTypes.LoadByOrganizationID(_organizationID);
        }
        else
        {
          phoneTypeID = phoneType.PhoneTypeID;
        }
        newPhoneNumber.PhoneTypeID = phoneTypeID;

        if (!string.IsNullOrEmpty(phoneTypeName2))
        {
          PhoneType phoneType2 = phoneTypes.FindByName(phoneTypeName2);
          if (phoneType2 == null)
          {
            PhoneTypes newPhoneTypes = new PhoneTypes(_importUser);
            PhoneType newPhoneType = newPhoneTypes.AddNewPhoneType();
            newPhoneType.Name = phoneTypeName;
            newPhoneType.OrganizationID = _organizationID;
            if (dateCreated != null)
            {
              newPhoneType.DateCreated = (DateTime)dateCreated;
            }
            newPhoneType.CreatorID = creatorID;
            newPhoneTypes.Save();
            phoneTypeID = newPhoneType.PhoneTypeID;

            phoneTypes = new PhoneTypes(_importUser);
            phoneTypes.LoadByOrganizationID(_organizationID);
          }
          else
          {
            phoneTypeID = phoneType2.PhoneTypeID;
          }
          newPhoneNumber2.PhoneTypeID = phoneTypeID;
        }

        if (!string.IsNullOrEmpty(phoneTypeName3))
        {
          PhoneType phoneType3 = phoneTypes.FindByName(phoneTypeName3);
          if (phoneType3 == null)
          {
            PhoneTypes newPhoneTypes = new PhoneTypes(_importUser);
            PhoneType newPhoneType = newPhoneTypes.AddNewPhoneType();
            newPhoneType.Name = phoneTypeName;
            newPhoneType.OrganizationID = _organizationID;
            if (dateCreated != null)
            {
              newPhoneType.DateCreated = (DateTime)dateCreated;
            }
            newPhoneType.CreatorID = creatorID;
            newPhoneTypes.Save();
            phoneTypeID = newPhoneType.PhoneTypeID;

            phoneTypes = new PhoneTypes(_importUser);
            phoneTypes.LoadByOrganizationID(_organizationID);
          }
          else
          {
            phoneTypeID = phoneType3.PhoneTypeID;
          }
          newPhoneNumber3.PhoneTypeID = phoneTypeID;
        }

        newPhoneNumber.Number = ReadString("Number");
        newPhoneNumber2.Number = ReadString("Number2");
        newPhoneNumber3.Number = ReadString("Number3");

        newPhoneNumber.Extension = ReadString("Extension");
        newPhoneNumber2.Extension = ReadString("Extension2");
        newPhoneNumber3.Extension = ReadString("Extension3");

        PhoneNumbers existingPhoneNumbers = new PhoneNumbers(_importUser);
        existingPhoneNumbers.LoadByID(newPhoneNumber.RefID, phoneNumberReferenceType);
        bool alreadyExists = false;
        bool alreadyExists2 = false;
        bool alreadyExists3 = false;
        bool phoneAdded = false;

        foreach (PhoneNumber existingPhoneNumber in existingPhoneNumbers)
        {
          if (
            newPhoneNumber.Number.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Number.Replace(" ", string.Empty).ToLower()
            && newPhoneNumber.Extension.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Extension.Replace(" ", string.Empty).ToLower()
            && newPhoneNumber.PhoneTypeID == existingPhoneNumber.PhoneTypeID)
          {
            alreadyExists = true;
            break;
          }
        }

        if (!alreadyExists && (newPhoneNumber.Number.Trim() != string.Empty || newPhoneNumber.Extension.Trim() != string.Empty))
        {
          phoneAdded = true;
          bulkCount++;
          PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
          phoneNumber.RefType = newPhoneNumber.RefType;
          phoneNumber.DateCreated = newPhoneNumber.DateCreated;
          phoneNumber.CreatorID = newPhoneNumber.CreatorID;
          phoneNumber.ModifierID = newPhoneNumber.ModifierID;
          phoneNumber.RefID = newPhoneNumber.RefID;
          phoneNumber.Number = newPhoneNumber.Number;
          phoneNumber.Extension = newPhoneNumber.Extension;
          phoneNumber.PhoneTypeID = newPhoneNumber.PhoneTypeID;
          _importLog.Write("Phone Number in row " + index.ToString() + " was added to phone numbers set.");
        }
        else
        {
          _importLog.Write("Phone Number in row " + index.ToString() + " already exists and was not added to phone numbers set.");
        }

        if (!string.IsNullOrEmpty(newPhoneNumber2.Number))
        {
          foreach (PhoneNumber existingPhoneNumber in existingPhoneNumbers)
          {
            if (
              newPhoneNumber2.Number.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Number.Replace(" ", string.Empty).ToLower()
              && newPhoneNumber2.Extension.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Extension.Replace(" ", string.Empty).ToLower()
              && newPhoneNumber2.PhoneTypeID == existingPhoneNumber.PhoneTypeID)
            {
              alreadyExists2 = true;
              break;
            }
          }

          if (!alreadyExists2 && (newPhoneNumber2.Number.Trim() != string.Empty || newPhoneNumber2.Extension.Trim() != string.Empty))
          {
            phoneAdded = true;
            bulkCount++;
            PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
            phoneNumber.RefType = newPhoneNumber2.RefType;
            phoneNumber.DateCreated = newPhoneNumber2.DateCreated;
            phoneNumber.CreatorID = newPhoneNumber2.CreatorID;
            phoneNumber.ModifierID = newPhoneNumber2.ModifierID;
            phoneNumber.RefID = newPhoneNumber2.RefID;
            phoneNumber.Number = newPhoneNumber2.Number;
            phoneNumber.Extension = newPhoneNumber2.Extension;
            phoneNumber.PhoneTypeID = newPhoneNumber2.PhoneTypeID;
            _importLog.Write("Phone Number 2 in row " + index.ToString() + " was added to phone numbers set.");
          }
          else
          {
            _importLog.Write("Phone Number 2 in row " + index.ToString() + " already exists and was not added to phone numbers set.");
          }
        }

        if (!string.IsNullOrEmpty(newPhoneNumber3.Number))
        {
          foreach (PhoneNumber existingPhoneNumber in existingPhoneNumbers)
          {
            if (
              newPhoneNumber3.Number.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Number.Replace(" ", string.Empty).ToLower()
              && newPhoneNumber3.Extension.Replace(" ", string.Empty).ToLower() == existingPhoneNumber.Extension.Replace(" ", string.Empty).ToLower()
              && newPhoneNumber3.PhoneTypeID == existingPhoneNumber.PhoneTypeID)
            {
              alreadyExists3 = true;
              break;
            }
          }

          if (!alreadyExists3 && (newPhoneNumber3.Number.Trim() != string.Empty || newPhoneNumber3.Extension.Trim() != string.Empty))
          {
            phoneAdded = true;
            bulkCount++;
            PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();
            phoneNumber.RefType = newPhoneNumber3.RefType;
            phoneNumber.DateCreated = newPhoneNumber3.DateCreated;
            phoneNumber.CreatorID = newPhoneNumber3.CreatorID;
            phoneNumber.ModifierID = newPhoneNumber3.ModifierID;
            phoneNumber.RefID = newPhoneNumber3.RefID;
            phoneNumber.Number = newPhoneNumber3.Number;
            phoneNumber.Extension = newPhoneNumber3.Extension;
            phoneNumber.PhoneTypeID = newPhoneNumber3.PhoneTypeID;
            _importLog.Write("Phone Number 3 in row " + index.ToString() + " was added to phone numbers set.");
          }
          else
          {
            _importLog.Write("Phone Number 3 in row " + index.ToString() + " already exists and was not added to phone numbers set.");
          }
        }
        if (phoneAdded)
        {
          count++;
        }

        if (bulkCount % BULK_LIMIT == 0)
        {
          phoneNumbers.BulkSave();
          phoneNumbers = new PhoneNumbers(_importUser);
          UpdateImportCount(import, count);
          _importLog.Write("Import set with " + count.ToString() + " phone numbers inserted in database.");
        }
      }
      phoneNumbers.BulkSave();
      UpdateImportCount(import, count);
      _importLog.Write("Import set with " + count.ToString() + " phone numbers inserted in database.");
    }

    private void ImportTickets(Import import)
    {
      SortedList<string, int> userList = GetUserAndContactList();
      SortedList<string, int> userOnlyList = GetUserList();

      TicketTypes ticketTypes = new TicketTypes(_importUser);
      ticketTypes.LoadAllPositions(_organizationID);

      TicketStatuses ticketStatuses = new TicketStatuses(_importUser);
      ticketStatuses.LoadByOrganizationID(_organizationID);

      TicketSeverities ticketSeverities = new TicketSeverities(_importUser);
      ticketSeverities.LoadByOrganizationID(_organizationID);

      Groups groups = new Groups(_importUser);
      groups.LoadByOrganizationID(_organizationID);

      Organizations account = new Organizations(_importUser);
      account.LoadByOrganizationID(_organizationID);

      Products products = new Products(_importUser);
      products.LoadByOrganizationID(_organizationID);

      ProductVersions productVersions = new ProductVersions(_importUser);
      productVersions.LoadByParentOrganizationID(_organizationID);

      ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(_importUser);
      productVersionStatuses.LoadByOrganizationID(_organizationID);

      KnowledgeBaseCategories kbCats = new KnowledgeBaseCategories(_loginUser);
      kbCats.LoadAllCategories(_organizationID);

      Tickets tickets = new Tickets(_importUser);

      int maxTicketNumber = tickets.GetMaxTicketNumber(_organizationID);
      if (maxTicketNumber < 0) maxTicketNumber++;

      int count = 0;

      while (_csv.ReadNextRecord())
      {
        Tickets existingTicket = new Tickets(_importUser);
        Ticket ticket = null;
        bool isUpdate = false;

        string importID = ReadString("ImportID");
        if (importID != string.Empty)
        {
          existingTicket.LoadByImportID(importID, _organizationID);
          if (existingTicket.Count == 1)
          {
            ticket = existingTicket[0];
            isUpdate = true;
          }
          else if (existingTicket.Count > 1)
          {
            _importLog.Write("More than one action matching the importID was found.");
            continue;
          }
        }

        int? ticketNumber;
        if (ticket == null)
        {
          ticketNumber = ReadIntNull("TicketNumber");
          if (ticketNumber != null)
          {
            existingTicket = new Tickets(_importUser);
            existingTicket.LoadByTicketNumber(_organizationID, (int)ticketNumber);
            if (existingTicket.Count == 1)
            {
              ticket = existingTicket[0];
              isUpdate = true;
              maxTicketNumber = Math.Max((int)ticketNumber, maxTicketNumber);
            }
            else if (existingTicket.Count > 0)
            {
              _importLog.Write("More than one action matching the TicketNumber was found.");
              continue;
            }
          }
          else
          {
            ticketNumber = maxTicketNumber + 1;
            maxTicketNumber++;
          }
        }
        else
        {
          ticketNumber = ticket.TicketNumber;
        }

        if (ticket == null)
        {
          ticket = tickets.AddNewTicket();
        }
        ticket.TicketNumber = (int)ticketNumber;

        string name = ReadString("Name");
        if (string.IsNullOrEmpty(name))
        {
          if (!isUpdate)
          {
            _importLog.Write("Ticket skipped due to missing name");
            continue;
          }
        }
        else
        {
          ticket.Name = name;
        }

        int creatorID = -2;
        if (Int32.TryParse(ReadString("CreatorID"), out creatorID)) {
          if (!userList.ContainsValue(creatorID)){
            creatorID = -2;
          }
        }

        DateTime now = DateTime.UtcNow;

        TicketType ticketType = null;
        string ticketTypeString = ReadString("Type");
        if (string.IsNullOrEmpty(ticketTypeString))
        {
          if (!isUpdate)
          {
            _importLog.Write("Ticket skipped due to missing type");
            continue;
          }
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
            ticketType.CreatorID = creatorID;
            ticketType.ModifierID = -2;
            ticketType.DateCreated = now;
            ticketType.DateModified = now;
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
            newTicketStatus.CreatorID = creatorID;
            newTicketStatus.DateCreated = now;
            newTicketStatus.ModifierID = -2;
            newTicketStatus.DateModified = now;

            newTicketStatus = ticketStatuses.AddNewTicketStatus();
            newTicketStatus.Name = "Closed";
            newTicketStatus.Description = "Closed";
            newTicketStatus.Position = 30;
            newTicketStatus.OrganizationID = _organizationID;
            newTicketStatus.TicketTypeID = ticketType.TicketTypeID;
            newTicketStatus.IsClosed = true;
            newTicketStatus.IsClosedEmail = false;
            newTicketStatus.CreatorID = creatorID;
            newTicketStatus.DateCreated = now;
            newTicketStatus.ModifierID = -2;
            newTicketStatus.DateModified = now;
            newTicketStatus.Collection.Save();
            newTicketStatus.Collection.ValidatePositions(_organizationID);
          }
          ticket.TicketTypeID = ticketType.TicketTypeID;
        }

        TicketStatus ticketStatus = null;
        string ticketStatusString = ReadString("Status");
        if (string.IsNullOrEmpty(ticketStatusString))
        {
          if (!isUpdate)
          {
            _importLog.Write("Ticket skipped due to missing status");
            continue;
          }
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
            ticketStatus.CreatorID = creatorID;
            ticketStatus.DateCreated = now;
            ticketStatus.ModifierID = -2;
            ticketStatus.DateModified = now;
            ticketStatuses.Save();
            ticketStatuses.ValidatePositions(_organizationID);
          }
          ticket.TicketStatusID = ticketStatus.TicketStatusID;
        }

        TicketSeverity ticketSeverity = null;
        string ticketSeverityString = ReadString("Severity");
        if (string.IsNullOrEmpty(ticketSeverityString))
        {
          if (!isUpdate)
          {
            _importLog.Write("Ticket skipped due to missing severity");
            continue;
          }
        }
        else
        {
          ticketSeverity = ticketSeverities.FindByName(ticketSeverityString);
          if (ticketSeverity == null)
          {
            ticketSeverity = ticketSeverities.AddNewTicketSeverity();
            ticketSeverity.Name = ticketSeverityString;
            ticketSeverity.Description = ticketSeverityString;
            ticketSeverity.Position = ticketSeverities.GetMaxPosition(_organizationID) + 1;
            ticketSeverity.OrganizationID = _organizationID;
            ticketSeverity.CreatorID = creatorID;
            ticketSeverity.DateCreated = now;
            ticketSeverity.ModifierID = -2;
            ticketSeverity.DateModified = now;
            ticketSeverities.Save();
            ticketSeverities.ValidatePositions(_organizationID);
          }
          ticket.TicketSeverityID = ticketSeverity.TicketSeverityID;
        }

        string emailOfUserAssignedTo = ReadString("EmailOfUserAssignedTo");
        if (!string.IsNullOrEmpty(emailOfUserAssignedTo))
        {
          int userID;
          if (userOnlyList.TryGetValue(emailOfUserAssignedTo.ToUpper(), out userID))
          {
            ticket.UserID = userID;
          }
        }

        string groupName = ReadString("Group");
        if (!string.IsNullOrEmpty(groupName))
        {
          TeamSupport.Data.Group group = groups.FindByName(groupName);
          if (group == null)
          {
            group = groups.AddNewGroup();
            group.Name = groupName;
            group.Description = groupName;
            group.OrganizationID = _organizationID;
            group.CreatorID = creatorID;
            group.ModifierID = -2;
            group.DateCreated = now;
            group.DateModified = now;
            groups.Save();
          }
          ticket.GroupID = group.GroupID;
        }

        ticket.DueDate = ReadDateNull("DueDate");

        string productName = ReadString("Product");
        Product product;
        if (!string.IsNullOrEmpty(productName))
        {
          product = products.FindByName(productName);
          if (product == null)
          {
            product = products.AddNewProduct();
            product.Name = groupName;
            product.Description = groupName;
            product.OrganizationID = _organizationID;
            product.CreatorID = creatorID;
            product.ModifierID = -2;
            product.DateCreated = now;
            product.DateModified = now;
            products.Save();
          }
          ticket.ProductID = product.ProductID;
        }
        else if (!isUpdate && account[0].ProductRequired)
        {
          _importLog.Write("Product is required and missing.");
          continue;
        }

        string reportedVersionName = ReadString("ReportedVersion");
        ProductVersion reportedVersion;
        if (!string.IsNullOrEmpty(reportedVersionName) && ticket.ProductID != null)
        {
          reportedVersion = productVersions.FindByVersionNumber(reportedVersionName, (int)ticket.ProductID);
          if (reportedVersion == null)
          {
            reportedVersion = productVersions.AddNewProductVersion();
            reportedVersion.VersionNumber = reportedVersionName;
            reportedVersion.Description = reportedVersionName;
            reportedVersion.ProductID = (int)ticket.ProductID;
            reportedVersion.ProductVersionStatusID = productVersionStatuses[0].ProductVersionStatusID;
            reportedVersion.IsReleased = true;
            reportedVersion.CreatorID = creatorID;
            reportedVersion.ModifierID = -2;
            reportedVersion.DateCreated = now;
            reportedVersion.DateModified = now;
            productVersions.Save();
          }
          ticket.ReportedVersionID = reportedVersion.ProductVersionID;
        }
        else if (!isUpdate && account[0].ProductVersionRequired)
        {
          _importLog.Write("Reported Version is required and missing.");
          continue;
        }

        string resolvedVersionName = ReadString("ResolvedVersion");
        ProductVersion resolvedVersion;
        if (!string.IsNullOrEmpty(resolvedVersionName) && ticket.ProductID != null)
        {
          resolvedVersion = productVersions.FindByVersionNumber(resolvedVersionName, (int)ticket.ProductID);
          if (resolvedVersion == null)
          {
            resolvedVersion = productVersions.AddNewProductVersion();
            resolvedVersion.VersionNumber = resolvedVersionName;
            resolvedVersion.Description = resolvedVersionName;
            resolvedVersion.ProductID = (int)ticket.ProductID;
            resolvedVersion.ProductVersionStatusID = productVersionStatuses[0].ProductVersionStatusID;
            resolvedVersion.IsReleased = true;
            resolvedVersion.CreatorID = creatorID;
            resolvedVersion.ModifierID = -2;
            resolvedVersion.DateCreated = now;
            resolvedVersion.DateModified = now;
            productVersions.Save();
          }
          ticket.SolvedVersionID = resolvedVersion.ProductVersionID;
        }

        ticket.IsKnowledgeBase = ReadBool("Knowledgebase");
        string parentCatName = ReadString("KBParentCatName");
        string catName = ReadString("KBCatName");
        KnowledgeBaseCategory cat = null;
        if (catName != null)
        {
          if (parentCatName == null)
          {
            cat = kbCats.FindByName(catName, -1);
            if (cat == null)
            {
              // craete cat
              AddKnowledgeBaseCategory(null, catName);
              kbCats = new KnowledgeBaseCategories(_loginUser);
              kbCats.LoadAllCategories(_organizationID);
              cat = kbCats.FindByName(catName, -1);
            }
          }
          else
          {
            KnowledgeBaseCategory parent = kbCats.FindByName(parentCatName, -1);
            if (parent != null)
            {
              cat = kbCats.FindByName(catName, parent.CategoryID);
            }
            else
            {
              // create parent 
              KnowledgeBaseCategory parentCat = AddKnowledgeBaseCategory(null, parentCatName);
              AddKnowledgeBaseCategory(parentCat.CategoryID, catName);
              kbCats = new KnowledgeBaseCategories(_loginUser);
              kbCats.LoadAllCategories(_organizationID);
              cat = kbCats.FindByName(catName, parentCat.CategoryID);
            }
          }
        }

        if (cat != null)
        {
          ticket.KnowledgeBaseCategoryID = cat.CategoryID;
        }

        ticket.IsVisibleOnPortal = ReadBool("VisibleToCustomers");
        ticket.OrganizationID = _organizationID;
        ticket.TicketSource = ReadString("Source");
        ticket.ImportID = ReadString("ImportID");
        DateTime? dateCreated = ReadDateNull("DateCreated");
        if (dateCreated != null)
        {
          ticket.DateCreated = (DateTime)dateCreated;
        }
        else
        {
          ticket.DateCreated = now;
        }
        ticket.CreatorID = creatorID;
        ticket.DateModified = now;
        ticket.ModifierID = -2;

        if (isUpdate)
        {
          existingTicket.Save();
          _importLog.Write("TicketID " + ticket.TicketID.ToString() + " was updated.");
        }
        count++;

        if (count % BULK_LIMIT == 0)
        {
          tickets.BulkSave();
          tickets = new Tickets(_importUser);
          UpdateImportCount(import, count);
          EmailPosts.DeleteImportEmails(_importUser);
        }
      }
      tickets.BulkSave();
      UpdateImportCount(import, count);
      EmailPosts.DeleteImportEmails(_importUser);
      _importLog.Write(count.ToString() + " tickets imported.");
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
        if (!fields.TryGetValue(apiFieldName.ToUpper(), out list))
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
      ImportFieldsViewItem map = _map.FindByFieldName(field);
      if (map != null && map.SourceName.Trim() != string.Empty)
      {
        return map.SourceName.Trim();
      }
      else
      {
        return field;
      }
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

    private SortedList<string, int> GetAssetList(SortedList<string, int> list = null)
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
SELECT DISTINCT(REPLACE(a.SerialNumber + a.Name + a.Location, ' ', '')), MAX(a.AssetID)
FROM Assets a 
WHERE a.OrganizationID = @OrganizationID
GROUP BY REPLACE(a.SerialNumber + a.Name + a.Location, ' ', '')";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", _organizationID);
      return list == null ? GetList(command) : GetList(command, list);
    }

    private SortedList<string, int> GetContactList(SortedList<string, int> list = null)
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

    private SortedList<string, int> GetCompanyList(SortedList<string, int> list = null)
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
SELECT DISTINCT(REPLACE(o.Name, ' ', '')), MAX(o.OrganizationID)
FROM Organizations o
WHERE (o.ParentID = @OrganizationID)
GROUP BY o.Name";
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

    public KnowledgeBaseCategory AddKnowledgeBaseCategory(int? parentID, string name)
    {
      KnowledgeBaseCategory cat = (new KnowledgeBaseCategories(_loginUser)).AddNewKnowledgeBaseCategory();
      cat.OrganizationID = _organizationID;
      cat.CategoryName = name;
      cat.ParentID = parentID ?? -1;
      cat.Position = GetKnowledgeBaseCategoryMaxPosition(parentID) + 1;
      cat.VisibleOnPortal = true;
      cat.Collection.Save();
      return cat;
    }

    private int GetKnowledgeBaseCategoryMaxPosition(int? parentID)
    {
      parentID = parentID ?? -1;

      KnowledgeBaseCategories cats = new KnowledgeBaseCategories(_loginUser);
      if (parentID < 0) cats.LoadCategories(_organizationID);
      else cats.LoadSubcategories((int)parentID);

      int max = -1;

      foreach (KnowledgeBaseCategory cat in cats)
      {
        if (cat.Position != null && cat.Position > max) max = (int)cat.Position;
      }

      return max;
    }
  }

  public class ImportLog
  {
    private string _logPath;
    private string _fileName;

    public ImportLog(string path, int importID)
    {
      _logPath = path;
      _fileName = importID.ToString() + ".txt";

      if (!Directory.Exists(_logPath))
      {
        Directory.CreateDirectory(_logPath);
      }
    }

    public void Write(string text)
    {
      if (!File.Exists(_logPath + @"\" + _fileName))
      {
        foreach (string oldFileName in Directory.GetFiles(_logPath))
        {
          if (File.GetLastWriteTime(oldFileName).AddDays(30) < DateTime.Today)
          {
            File.Delete(oldFileName);
          }
        }
      }

      File.AppendAllText(_logPath + @"\" + _fileName, DateTime.Now.ToLongTimeString() + ": " + text + Environment.NewLine);
    }
  }
}
