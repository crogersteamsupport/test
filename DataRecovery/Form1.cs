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

		private Logs _logs;
		private string _importID;
    private Users _users;

		public Form1()
		{
			InitializeComponent();

			
		}

		private LoginUser GetCorrupteLoginUser()
		{ 
		   return new LoginUser("Data Source=10.42.42.105; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=500", -1, -1, null);
		}

		private LoginUser GetGoodLoginUser()
		{
			return new LoginUser("Data Source=10.42.42.101; Initial Catalog=TeamSupport;Persist Security Info=True;User ID=webuser;Password=3209u@j#*29;Connect Timeout=500", -1, -1, null);
		}


		private int GetNextOrg()
		{
		  return SqlExecutor.ExecuteInt(GetCorrupteLoginUser(), "select top 1 OrganizationID from OrgMoveEvent where HasExecuted = 0 order by DateCreated");
      }



		private void button1_Click(object sender, EventArgs e)
		{
		   int orgID = GetNextOrg();

			while (orgID > -1)
			{ 
				if (orgID < 0) return;

        try
        {
          _importID = orgID.ToString() + "-" + new Guid();
          _logs = new Logs(orgID.ToString() + " - Org.txt");
          _users = new Users(GetGoodLoginUser());
          _users.LoadByOrganizationID(orgID, false);
          RecoverCompanies(orgID);
          //RecoverContacts(orgID);
          RecoverProducts(orgID);
         // RecoverAssets(orgID);
          RecoverActionsFromOldTickets(orgID);
          RecoverTickets(orgID);

          SaveOrg(orgID, "Success");
          SqlExecutor.ExecuteNonQuery(GetGoodLoginUser(), "update organizations set LastIndexRebuilt='1/1/2000' where OrganizationID=" + orgID.ToString());
        }
        catch (Exception  ex)
        {
          SaveOrg(orgID, "Failure: " + ex.Message);
          ExceptionLogs.LogException(GetCorrupteLoginUser(), ex, "recover");
        }
		  }
		}

		private void SaveOrg(int orgID, string result)
		{
			SqlExecutor.ExecuteNonQuery(GetCorrupteLoginUser(), "UPDATE OrgMoveEvent SET Result = '"+result+"', HasExecuted = 1 WHERE OrganizationID = " + orgID.ToString());
		}

		private void RecoverProducts(int orgID)
		{ 
		// check corrupt db for different products,if so craete the new products, but do not use ID's
		  Products badProducts = new Products(GetCorrupteLoginUser());
		  badProducts.LoadByOrganizationID(orgID);

		  Products goodProducts = new Products(GetGoodLoginUser());
		  goodProducts.LoadByOrganizationID(orgID);

		  foreach (Product badProduct in badProducts)
		  {
			  Product goodProduct = goodProducts.FindByName(badProduct.Name);
			  if (goodProduct == null)
			  { 
			    goodProduct = (new Products(GetGoodLoginUser())).AddNewProduct();
				 goodProduct.Name = badProduct.Name;
				 goodProduct.DateCreated = badProduct.DateCreated;
          if (badProduct.CreatorID > 0)
          {
            User creator = _users.FindByUserID(badProduct.CreatorID);
            if (creator != null)
            {
              goodProduct.CreatorID = creator.UserID;
            }
            else
            {
              goodProduct.CreatorID = -1;
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
		}

    private void RecoverCompanies(int orgID)
    {
      Organizations badCompanies = new Organizations(GetCorrupteLoginUser());
      badCompanies.LoadByParentID(orgID, false);

      Organizations goodCompanies = new Organizations(GetGoodLoginUser());
      goodCompanies.LoadByParentID(orgID, false);

      foreach (Organization badCompany in badCompanies)
      {
        Organization goodCompany = goodCompanies.FindByName(badCompany.Name);
        if (goodCompany == null)
        {
          goodCompany = (new Organizations(GetGoodLoginUser())).AddNewOrganization();
          goodCompany.CopyRowData(badCompany);
          goodCompany.ImportID = _importID;
          goodCompany.ParentID = orgID;
          goodCompany.Collection.Save();
        }
      }
    }

    //private void RecoverContacts(int orgID)
    //{
    //  // check corrupt db for different products,if so craete the new products, but do not use ID's
    //  Users badContacts = new Users(GetCorrupteLoginUser());
    //  badContacts.LoadContacts(orgID, false);

    //  Users goodContacts = new Users(GetGoodLoginUser());
    //  goodContacts.LoadContacts(orgID, false);

    //  foreach (User badContact in badContacts)
    //  {
    //    User goodContact = goodContacts.FindBy(badCompany.Name);
    //    if (goodCompany == null)
    //    {
    //      goodCompany = (new Organizations(GetGoodLoginUser())).AddNewOrganization();
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

    private void RecoverAssets(int orgID)
    {
      // check corrupt db for different products,if so craete the new products, but do not use ID's
      Assets badAssets = new Assets(GetCorrupteLoginUser());
      badAssets.LoadByOrganizationIDCreatedAfterRestore(orgID);

      Assets goodAssets = new Assets(GetGoodLoginUser());
      goodAssets.LoadByOrganizationID(orgID);

      foreach (Asset badAsset in badAssets)
      {
        Asset goodAsset = goodAssets.FindByName(badAsset.Name);
        try
        {
          if (goodAsset == null)
          {
            goodAsset = (new Assets(GetGoodLoginUser())).AddNewAsset();
            goodAsset.CopyRowData(badAsset);
            goodAsset.OrganizationID = orgID;
            goodAsset.ImportID = _importID;
            goodAsset.Collection.Save();
            if (goodAsset.Location == "1")
            {
              AssetAssignmentsView assetAssignments = new AssetAssignmentsView(GetCorrupteLoginUser());
              assetAssignments.LoadByAssetID(badAsset.AssetID);
              foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
              {
                AssetHistory assetHistory = new AssetHistory(GetGoodLoginUser());
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

                assetHistoryItem.DateCreated = assetAssignment.DateCreated;
                assetHistoryItem.Actor = assetAssignment.Actor;
                assetHistoryItem.RefType = assetAssignment.RefType;
                assetHistoryItem.DateModified = now;

                assetHistory.Save();

                AssetAssignments goodAssetAssignments = new AssetAssignments(GetGoodLoginUser());
                AssetAssignment goodAssetAssignment = goodAssetAssignments.AddNewAssetAssignment();

                goodAssetAssignment.HistoryID = assetHistoryItem.HistoryID;

                goodAssetAssignments.Save();
              }
            }
          }

        }
        catch (Exception)
        {
          
        }
      }
    }

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
        TeamSupport.Data.Action goodAction = new Actions(GetGoodLoginUser()).AddNewAction();
        goodAction.CopyRowData(badAction);
        goodAction.TicketID = badAction.TicketID;
        goodAction.ModifierID = -5;
        goodAction.Collection.Save();

      }

    }

    private void RecoverTickets(int orgID)
    {
      Tickets badTickets = new Tickets(GetCorrupteLoginUser());
      SqlCommand command = new SqlCommand();
      command.CommandText = "SELECT * FROM Tickets WHERE (OrganizationID = @OrganizationID) AND DateCreated > '2015-09-17 05:56:00'";
      command.CommandType = CommandType.Text;
      command.Parameters.AddWithValue("@OrganizationID", orgID);
      badTickets.Fill(command, "");


      foreach (Ticket badTicket in badTickets)
      {
        Ticket goodTicket = (new Tickets(GetGoodLoginUser())).AddNewTicket();
        goodTicket.CopyRowData(badTicket);
        goodTicket.ParentID = null;
        goodTicket.ImportID = _importID;
        goodTicket.ModifierID = -5;
        goodTicket.TicketNumber = 0;
        goodTicket.Collection.Save();

        Actions badActions = new Actions(GetCorrupteLoginUser());
        badActions.LoadByTicketID(badTicket.TicketID);

        foreach (TeamSupport.Data.Action badAction in badActions)
        {
          TeamSupport.Data.Action goodAction = new Actions(GetGoodLoginUser()).AddNewAction();
          goodAction.CopyRowData(badAction);
          goodAction.TicketID = goodTicket.TicketID;
          goodAction.ModifierID = -5;
          goodAction.Collection.Save();

        }


        Organizations orgs = new Organizations(GetGoodLoginUser());
        orgs.LoadBTicketID(badTicket.TicketID);

        foreach (Organization org in orgs)
        {
          if (org.ParentID == orgID)
          {
            goodTicket.Collection.AddOrganization(org.OrganizationID, goodTicket.TicketID);
          }
        }

        RecoverTicketCustomValues(orgID, badTicket.TicketID, goodTicket.TicketID);
      }
    }

    private void RecoverTicketCustomValues(int orgID, int badTicketID, int goodTicketID)
    {
      CustomValues badCustomValues = new CustomValues(GetCorrupteLoginUser());
      badCustomValues.LoadByReferenceTypeModifiedAfterRecovery(orgID, ReferenceType.Tickets, badTicketID);

      foreach (CustomValue badCustomValue in badCustomValues)
      {
        if (badCustomValue == null) continue;
        CustomValue goodCustomValue = CustomValues.GetValue(GetGoodLoginUser(), goodTicketID, badCustomValue.ApiFieldName);
        if (goodCustomValue != null)
        {
          goodCustomValue.Value = badCustomValue.Value;
          goodCustomValue.Collection.Save();
        }
      }
    }
  }
}
