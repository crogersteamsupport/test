using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamSupport.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DataRecovery
{
  public partial class Form1 : Form
  {

    public class OrgComboItem
    {
      public OrgComboItem(int orgID, string name)
      {
        this._name = name;
        this._organizationID = orgID;
      }

      private int _organizationID;

      public int OrganizationID
      {
        get { return _organizationID; }
        set { _organizationID = value; }
      }
      private string _name;

      public string Name
      {
        get { return _name; }
        set { _name = value; }
      }

    }


    private Logs _logs;
    private string _importID;
    private Users _users;

    public Form1()
    {
      InitializeComponent();
      LoadOrgCombo();

    }

    private void LoadOrgCombo()
    {

      SqlCommand command = new SqlCommand();
      command.CommandText = @"
        SELECT OrganizationID, Name FROM Organizations WHERE ParentID=1 AND IsActive=1 ORDER BY Name";
      DataTable table = SqlExecutor.ExecuteQuery(GetCorrupteLoginUser(), command);
      cmbOrg.BeginUpdate();
      foreach (DataRow row in table.Rows)
      {
        cmbOrg.Items.Add(new OrgComboItem((int)row[0], row[1].ToString()));
      }
      cmbOrg.EndUpdate();

    }

    private LoginUser GetCorrupteLoginUser()
    {
      return new LoginUser("Data Source=10.42.42.105; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=500", -5, -1, null);
    }

    private LoginUser GetReviewLoginUser()
    {
      return new LoginUser("Data Source=10.42.42.105; Initial Catalog=TeamSupportTest;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=500", -5, -1, null);
    }

    private LoginUser GetPRODUCTIONLoginUser()
    {
      return new LoginUser("Data Source=10.42.42.101; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=500", -5, -1, null);
    }

    private int GetNextOrg()
    {
      return SqlExecutor.ExecuteInt(GetCorrupteLoginUser(), @"
        SELECT
          TOP 1 OrganizationID 
        FROM
          OrgMoveEvent
        WHERE
          HasExecuted = 0 
        ORDER BY
          Priority
      ");
    }

    private void RollBack(int orgID, LoginUser loginUser)
    {
      // leo's rollback code

      SaveOrgResults(orgID, "RolledBack", false);
    }

    private void ImportOrg(int orgID, LoginUser loginUser)
    {
      try
      {
        _importID = orgID.ToString() + "-" + Guid.NewGuid().ToString();
        _logs = new Logs(orgID.ToString() + " - Org.txt");
        _users = new Users(loginUser);
        _users.LoadByOrganizationID(orgID, false);
        if (cbCompanies.Checked) RecoverCompanies(orgID);
        //RecoverContacts(orgID);
        if (cbProducts.Checked) RecoverProducts(orgID);
        // RecoverAssets(orgID);
        if (cbOldActions.Checked) RecoverActionsFromOldTickets(orgID);
        if (cbTickets.Checked) RecoverTickets(orgID);

        SaveOrgResults(orgID, "Success");
        SqlExecutor.ExecuteNonQuery(GetReviewLoginUser(), "update organizations set LastIndexRebuilt='1/1/2000' where OrganizationID=" + orgID.ToString());
      }
      catch (Exception ex)
      {
        SaveOrgResults(orgID, "Failure: " + ex.Message);
        ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
      }


    }

    private void SaveOrgResults(int orgID, string result, bool hasExectued = true)
    {
      SqlCommand command = new SqlCommand();
      command.CommandText = "UPDATE OrgMoveEvent SET Result = @Result, HasExecuted = @HasExecuted WHERE OrganizationID = @OrganizationID";
      command.Parameters.AddWithValue("Result", result);
      command.Parameters.AddWithValue("HasExecuted", hasExectued);
      command.Parameters.AddWithValue("OrganizationID", orgID);
      SqlExecutor.ExecuteNonQuery(GetCorrupteLoginUser(), command);
    }

    private void RecoverProducts(int orgID)
    {
      // check corrupt db for different products,if so craete the new products, but do not use ID's
      Products badProducts = new Products(GetCorrupteLoginUser());
      badProducts.LoadByOrganizationID(orgID);

      Products goodProducts = new Products(GetReviewLoginUser());
      goodProducts.LoadByOrganizationID(orgID);

      foreach (Product badProduct in badProducts)
      {
        try
        {
          Product goodProduct = goodProducts.FindByName(badProduct.Name);
          if (goodProduct == null)
          {
            goodProduct = (new Products(GetReviewLoginUser())).AddNewProduct();
            goodProduct.Name = badProduct.Name;
            goodProduct.DateCreated = badProduct.DateCreatedUtc;
            if (badProduct.CreatorID > 0)
            {
              User creator = _users.FindByUserID(badProduct.CreatorID);
              if (creator != null)
              {
                goodProduct.CreatorID = creator.UserID;
              }
              else
              {
                goodProduct.CreatorID = -5;
              }
            }
            else
            {
              goodProduct.CreatorID = badProduct.CreatorID;
            }
            goodProduct.OrganizationID = orgID;
            goodProduct.ImportID = _importID;
            goodProduct.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          SaveOrgResults(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }

    private void RecoverCompanies(int orgID)
    {
      Organizations badCompanies = new Organizations(GetCorrupteLoginUser());
      badCompanies.LoadByParentID(orgID, false);

      Organizations goodCompanies = new Organizations(GetReviewLoginUser());
      goodCompanies.LoadByParentID(orgID, false);

      foreach (Organization badCompany in badCompanies)
      {
        try
        {
          Organization goodCompany = goodCompanies.FindByName(badCompany.Name);
          if (goodCompany == null)
          {
            goodCompany = (new Organizations(GetReviewLoginUser())).AddNewOrganization();
            goodCompany.CopyRowData(badCompany);
            goodCompany.DateCreated = badCompany.DateCreatedUtc;
            goodCompany.DateModified = badCompany.DateModifiedUtc;
            goodCompany.ImportID = _importID;
            goodCompany.ParentID = orgID;
            goodCompany.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          SaveOrgResults(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }

    //private void RecoverContacts(int orgID)
    //{
    //  // check corrupt db for different products,if so craete the new products, but do not use ID's
    //  Users badContacts = new Users(GetCorrupteLoginUser());
    //  badContacts.LoadContacts(orgID, false);

    //  Users goodContacts = new Users(GetReviewLoginUser());
    //  goodContacts.LoadContacts(orgID, false);

    //  foreach (User badContact in badContacts)
    //  {
    //    User goodContact = goodContacts.FindBy(badCompany.Name);
    //    if (goodCompany == null)
    //    {
    //      goodCompany = (new Organizations(GetReviewLoginUser())).AddNewOrganization();
    //      goodCompany.Name = badCompany.Name;
    //      goodCompany.Description = badCompany.Description;
    //      goodCompany.Website = badCompany.Website;
    //      goodCompany.CompanyDomains = badCompany.CompanyDomains;
    //      goodCompany.SAExpirationDate = badCompany.SAExpirationDate;
    //      goodCompany.SlaLevelID = badCompany.SlaLevelID;
    //      goodCompany.DateCreated = badCompany.DateCreated;
    //      goodCompany.SupportHoursMonth = badCompany.SupportHoursMonth;
    //      goodCompany.IsActive = badCompany.IsActive;
    //      goodCompany.HasPortalAccess = badCompany.HasPortalAccess;
    //      goodCompany.IsApiEnabled = badCompany.IsApiEnabled;
    //      goodCompany.IsApiActive = badCompany.IsApiActive;
    //      goodCompany.InActiveReason = badCompany.InActiveReason;

    //      goodCompany.ExtraStorageUnits = badCompany.ExtraStorageUnits;
    //      goodCompany.IsCustomerFree = badCompany.IsCustomerFree;
    //      goodCompany.PortalSeats = badCompany.PortalSeats;
    //      goodCompany.ProductType = badCompany.ProductType;
    //      goodCompany.UserSeats = badCompany.UserSeats;
    //      goodCompany.NeedsIndexing = badCompany.NeedsIndexing;
    //      goodCompany.SystemEmailID = badCompany.SystemEmailID;
    //      goodCompany.WebServiceID = badCompany.WebServiceID;
    //      {
    //        User defaultSupportUser = _users.FindByUserID((int)badCompany.DefaultSupportUserID);
    //        if (defaultSupportUser != null)
    //        {
    //          goodCompany.DefaultSupportUserID = defaultSupportUser.UserID;
    //        }
    //      }
    //      if (badCompany.CreatorID > 0)
    //      {
    //        User creator = _users.FindByUserID(badCompany.CreatorID);
    //        if (creator != null)
    //        {
    //          goodCompany.CreatorID = creator.UserID;
    //        }
    //        else
    //        {
    //          goodCompany.CreatorID = -1;
    //        }
    //      }
    //      else
    //      {
    //        goodCompany.CreatorID = badCompany.CreatorID;
    //      }
    //      goodCompany.ParentID = orgID;
    //      goodCompany.ImportID = _importID;
    //      goodCompany.Collection.Save();
    //    }
    //  }
    //}
    /*
    private void RecoverAssets(int orgID)
    {
      // check corrupt db for different products,if so craete the new products, but do not use ID's
      Assets badAssets = new Assets(GetCorrupteLoginUser());
      badAssets.LoadByOrganizationIDCreatedAfterRestore(orgID);

      Assets goodAssets = new Assets(GetReviewLoginUser());
      goodAssets.LoadByOrganizationID(orgID);

      foreach (Asset badAsset in badAssets)
      {
        Asset goodAsset = goodAssets.FindByName(badAsset.Name);
        try
        {
          if (goodAsset == null)
          {
            goodAsset = (new Assets(GetReviewLoginUser())).AddNewAsset();
            goodAsset.CopyRowData(badAsset);
            goodAsset.DateCreated = badAsset.DateCreatedUtc;
            goodAsset.DateModified = badAsset.DateModifiedUtc;
            goodAsset.OrganizationID = orgID;
            goodAsset.ImportID = _importID;
            goodAsset.Collection.Save();
            if (goodAsset.Location == "1")
            {
              AssetAssignmentsView assetAssignments = new AssetAssignmentsView(GetCorrupteLoginUser());
              assetAssignments.LoadByAssetID(badAsset.AssetID);
              foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
              {
                AssetHistory assetHistory = new AssetHistory(GetReviewLoginUser());
                AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                DateTime now = DateTime.UtcNow;

                assetHistoryItem.AssetID = goodAsset.AssetID;
                assetHistoryItem.OrganizationID = orgID;
                assetHistoryItem.ActionTime = assetAssignment.ActionTime;
                assetHistoryItem.ActionDescription = assetAssignment.ActionDescription;
                assetHistoryItem.ShippedFrom = assetAssignment.ShippedFrom;
                assetHistoryItem.ShippedFromRefType = assetAssignment.ShippedFromRefType;
                assetHistoryItem.ShippedTo = assetAssignment.ShippedTo;
                assetHistoryItem.TrackingNumber = assetAssignment.TrackingNumber;
                assetHistoryItem.ShippingMethod = assetAssignment.ShippingMethod;
                assetHistoryItem.ReferenceNum = assetAssignment.ReferenceNum;
                assetHistoryItem.Comments = assetAssignment.Comments;

                assetHistoryItem.DateCreated = assetAssignment.DateCreatedUtc;
                assetHistoryItem.Actor = assetAssignment.Actor;
                assetHistoryItem.RefType = assetAssignment.RefType;
                assetHistoryItem.DateModified = now;

                assetHistory.Save();

                AssetAssignments goodAssetAssignments = new AssetAssignments(GetReviewLoginUser());
                AssetAssignment goodAssetAssignment = goodAssetAssignments.AddNewAssetAssignment();

                goodAssetAssignment.HistoryID = assetHistoryItem.HistoryID;

                goodAssetAssignments.Save();
              }
            }
          }

        }
        catch (Exception ex)
        {
          SaveOrgResults(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }
    */
    private void RecoverActionsFromOldTickets(int orgID)
    {
      Actions badActions = new Actions(GetCorrupteLoginUser());
      SqlCommand command = new SqlCommand();
      command.CommandText = @"SELECT a.*, t.TicketNumber, t.OrganizationID FROM Actions a
LEFT JOIN Tickets t ON t.TicketID = a.TicketID
WHERE (t.OrganizationID = @OrganizationID) 
AND a.DateCreated > '2015-09-17 05:56:00'
AND t.DateCreated < '2015-09-17 05:56:00'";

      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", orgID);
      badActions.Fill(command, "");

      foreach (TeamSupport.Data.Action badAction in badActions)
      {
        try
        {
          TeamSupport.Data.Action goodAction = new Actions(GetReviewLoginUser()).AddNewAction();
          goodAction.CopyRowData(badAction);
          goodAction.DateCreated = badAction.DateCreatedUtc;
          goodAction.DateModified = badAction.DateModifiedUtc;
          goodAction.TicketID = badAction.TicketID;
          if (badAction.CreatorID > 0)
          {
            User creator = _users.FindByUserID(badAction.CreatorID);
            if (creator != null)
            {
              goodAction.CreatorID = creator.UserID;
            }
            else
            {
              goodAction.CreatorID = -5;
            }
          }
          else
          {
            goodAction.CreatorID = badAction.CreatorID;
          }

          if (badAction.ModifierID > 0)
          {
            User modifier = _users.FindByUserID(badAction.ModifierID);
            if (modifier != null)
            {
              goodAction.ModifierID = modifier.UserID;
            }
            else
            {
              goodAction.ModifierID = -5;
            }
          }
          else
          {
            goodAction.ModifierID = badAction.ModifierID;
          }
          goodAction.ImportID = _importID;
          goodAction.Collection.Save();

          Ticket ticket = Tickets.GetTicket(GetReviewLoginUser(), goodAction.TicketID);
          ticket.ImportID = _importID;
          ticket.Collection.Save();

          EmailPosts.DeleteImportEmails(GetReviewLoginUser());
        }
        catch (Exception ex)
        {
          SaveOrgResults(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }

      }

    }

    private void RecoverTickets(int orgID)
    {
      Tickets badTickets = new Tickets(GetCorrupteLoginUser());
      SqlCommand command = new SqlCommand();
      command.CommandText = @"
        SELECT 
          * 
        FROM 
          Tickets 
        WHERE 
          OrganizationID = @OrganizationID
          AND DateCreated > '2015-09-17 05:56:00'
          AND TicketSource != 'EMail'
          AND IgnoreMe = 0'
      ";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", orgID);
      badTickets.Fill(command, "");

      Organizations existingCompanies = new Organizations(GetReviewLoginUser());
      existingCompanies.LoadByParentID(orgID, false);

      foreach (Ticket badTicket in badTickets)
      {
        try
        {
          Ticket goodTicket = (new Tickets(GetReviewLoginUser())).AddNewTicket();
          goodTicket.CopyRowData(badTicket);
          goodTicket.DateCreated = badTicket.DateCreatedUtc;
          goodTicket.DateModified = badTicket.DateModifiedUtc;
          goodTicket.ParentID = null;
          goodTicket.ImportID = _importID;
          if (badTicket.CreatorID > 0)
          {
            User creator = _users.FindByUserID(badTicket.CreatorID);
            if (creator != null)
            {
              goodTicket.CreatorID = creator.UserID;
            }
            else
            {
              goodTicket.CreatorID = -5;
            }
          }
          else
          {
            goodTicket.CreatorID = badTicket.CreatorID;
          }

          if (badTicket.ModifierID > 0)
          {
            User modifier = _users.FindByUserID(badTicket.ModifierID);
            if (modifier != null)
            {
              goodTicket.ModifierID = modifier.UserID;
            }
            else
            {
              goodTicket.ModifierID = -5;
            }
          }
          else
          {
            goodTicket.ModifierID = badTicket.ModifierID;
          }
          goodTicket.TicketNumber = 0;
          goodTicket.Collection.Save();
          EmailPosts.DeleteImportEmails(GetReviewLoginUser());

          Actions badActions = new Actions(GetCorrupteLoginUser());
          badActions.LoadByTicketID(badTicket.TicketID);

          foreach (TeamSupport.Data.Action badAction in badActions)
          {
            TeamSupport.Data.Action goodAction = new Actions(GetReviewLoginUser()).AddNewAction();
            goodAction.CopyRowData(badAction);
            goodAction.DateCreated = badAction.DateCreatedUtc;
            goodAction.DateModified = badAction.DateCreatedUtc;
            goodAction.TicketID = goodTicket.TicketID;
            goodAction.ImportID = _importID;
            if (badAction.CreatorID > 0)
            {
              User creator = _users.FindByUserID(badAction.CreatorID);
              if (creator != null)
              {
                goodAction.CreatorID = creator.UserID;
              }
              else
              {
                goodAction.CreatorID = -5;
              }
            }
            else
            {
              goodAction.CreatorID = badAction.CreatorID;
            }

            if (badAction.ModifierID > 0)
            {
              User modifier = _users.FindByUserID(badAction.ModifierID);
              if (modifier != null)
              {
                goodAction.ModifierID = modifier.UserID;
              }
              else
              {
                goodAction.ModifierID = -5;
              }
            }
            else
            {
              goodAction.ModifierID = badAction.ModifierID;
            }
            goodAction.Collection.Save();
            EmailPosts.DeleteImportEmails(GetReviewLoginUser());

          }


          Organizations orgs = new Organizations(GetCorrupteLoginUser());
          orgs.LoadBTicketID(badTicket.TicketID);

          foreach (Organization org in orgs)
          {
            Organization goodCompany = existingCompanies.FindByName(org.Name);
            if (org.ParentID == orgID && goodCompany != null)
            {
              goodTicket.Collection.AddOrganization(goodCompany.OrganizationID, goodTicket.TicketID);
              EmailPosts.DeleteImportEmails(GetReviewLoginUser());

            }
          }

          RecoverTicketCustomValues(orgID, badTicket.TicketID, goodTicket.TicketID);
          EmailPosts.DeleteImportEmails(GetReviewLoginUser());
        }
        catch (Exception ex)
        {
          SaveOrgResults(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }

      }
    }

    private void RecoverTicketCustomValues(int orgID, int badTicketID, int goodTicketID)
    {
      CustomValues badCustomValues = new CustomValues(GetCorrupteLoginUser());
      badCustomValues.LoadByReferenceTypeModifiedAfterRecovery(orgID, ReferenceType.Tickets, badTicketID);

      foreach (CustomValue badCustomValue in badCustomValues)
      {
        try
        {
          if (badCustomValue == null) continue;
          CustomValue goodCustomValue = CustomValues.GetValue(GetReviewLoginUser(), goodTicketID, badCustomValue.ApiFieldName);
          if (goodCustomValue != null)
          {
            goodCustomValue.Value = badCustomValue.Value;
            goodCustomValue.Collection.Save();
          }
        }
        catch (Exception ex)
        {
          SaveOrgResults(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      int orgID = GetNextOrg();

      while (orgID > -1)
      {
        if (orgID < 0) return;
        try
        {
          ImportOrg(orgID, GetReviewLoginUser());
        }
        finally
        {
          orgID = GetNextOrg();
        }

      }
    }

    private void btnRollBackAll_Click(object sender, EventArgs e)
    {
      int orgID = GetNextOrg();

      while (orgID > -1)
      {
        if (orgID < 0) return;
        try
        {
          RollBack(orgID, GetReviewLoginUser());
        }
        finally
        {
          orgID = GetNextOrg();
        }

      }
    }

    private void btnImportOrgToReview_Click(object sender, EventArgs e)
    {
      ImportOrg((cmbOrg.SelectedItem as OrgComboItem).OrganizationID, GetReviewLoginUser());
    }

    private void btnRollbackOrgFromReview_Click(object sender, EventArgs e)
    {
      RollBack((cmbOrg.SelectedItem as OrgComboItem).OrganizationID, GetReviewLoginUser());
    }

    private void btnImportOrgToProduction_Click(object sender, EventArgs e)
    {
      OrgComboItem orgitem = (cmbOrg.SelectedItem as OrgComboItem);

      string msg = string.Format("THIS WILL UPDATE '{0}:{1}' ON PRODUCTION.  Would you like to continue?", orgitem.Name, orgitem.OrganizationID.ToString());
      if (MessageBox.Show(msg, "Confrim", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
      ImportOrg(orgitem.OrganizationID, GetPRODUCTIONLoginUser());
    }

    private void btnRollBackOrgFromProduction_Click(object sender, EventArgs e)
    {
      OrgComboItem orgitem = (cmbOrg.SelectedItem as OrgComboItem);

      string msg = string.Format("THIS WILL ROLLBACK '{0}:{1}' ON PRODUCTION.  Would you like to continue?", orgitem.Name, orgitem.OrganizationID.ToString());
      if (MessageBox.Show(msg, "Confrim", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
      RollBack(orgitem.OrganizationID, GetPRODUCTIONLoginUser());
    }



  }
}
