using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using TeamSupport.Data;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace TeamSupport.DataManager
{
  public partial class MainForm : Form
  {
  
    public MainForm()
    {
      InitializeComponent();
    }

    private void radButton1_Click(object sender, EventArgs e)
    {
      //load all the tables

      //create sys data  
      //retrieve custom fields
      //validate custom fields
      //retrieve all of the types
      //validate types

      //validate dataset
        //set column data types
        

      //importdata(dataset, orgid)
      
        //map types
        //map custom fields
        //start transation
        
        /* Load:
         * Customers
         * Users
         * Products
         * Versions
         * Tickets
         * Actions
         * Contacts
         * Addresses
         * PhoneNumbers
         * CustomerProducts
         * CustomerTickets
         * CustomerNotes
         */
         
         //end transaxtion
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      int initOrgID = Properties.Settings.Default.LastOrgID;
      Properties.Settings.Default.Reload();
      Importer.SetTypeGuessRowsForExcelImport();
      LoadOrganizations();
      SelectOrganization(initOrgID);
    }
    
    private void LoadOrganizations()
    {
      cmbOrganization.Items.Clear();
      Organizations organizations = new Organizations(LoginSession.LoginUser);
      organizations.LoadByParentID(1, false);

      foreach (Organization organization in organizations)
      {
         cmbOrganization.Items.Add(new NamObjectItem(organization.Name + " [" + organization.OrganizationID.ToString() + "]", organization.OrganizationID));
      }
      cmbOrganization.SelectedIndex = 0;
    }
    
    private void SelectOrganization(int organizationID)
    {
      for (int i = 0; i < cmbOrganization.Items.Count; i++)
      {
        NamObjectItem item = (NamObjectItem)cmbOrganization.Items[i];
        if ((int)item.Value == organizationID)
        {
          cmbOrganization.SelectedIndex = i;
          return;
        }
      }
    }
    
    private void LoadOrganization(int organizationID)
    {
      Properties.Settings.Default.LastOrgID = organizationID;
      Properties.Settings.Default.Save();
      propertiesControl1.OrganizationID = organizationID;
      customFieldsControl1.OrganizationID = organizationID;
      customPropertiesControl1.OrganizationID = organizationID;
      importControl1.OrganizationID = organizationID;
    }

    private void btnDeleteOrganization_Click(object sender, EventArgs e)
    {
      Organizations organizations = new Organizations(LoginSession.LoginUser);
      organizations.LoadByOrganizationID((int)((NamObjectItem)cmbOrganization.SelectedItem).Value);

      if (organizations.IsEmpty)
      {
        MessageBox.Show("Invalid organization.");
        return;
      }

      Organization organization = organizations[0];
      if (MessageBox.Show("Are you sure you would like to delete this organization (" + organization.Name + ")?", "Delete Organization", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
      if (MessageBox.Show("Are you REALLY REALLY sure you would like to delete this organization (" + organization.Name + ")?", "Delete Organization", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

      try
      {
        Organizations.DeleteOrganizationAndAllReleatedData(LoginSession.LoginUser, organization.OrganizationID);
      }
      catch (Exception)
      {
        throw;// Organizations.DeleteOrganizationAndAllReleatedData(LoginSession.LoginUser, organization.OrganizationID);
      }

      LoadOrganizations();

    }

    private void cmbOrganization_SelectedIndexChanged(object sender, EventArgs e)
    {
      LoadOrganization((int)((NamObjectItem)cmbOrganization.SelectedItem).Value);
    }

    private void btnNewOrganization_Click(object sender, EventArgs e)
    {
      int organizationID = OrganizationForm.ShowOrganization();
      if (organizationID > -1)
      {
        LoadOrganizations();
        SelectOrganization(organizationID);
      }
    }

    private void btnEditOrganization_Click(object sender, EventArgs e)
    {
      OrganizationForm.ShowOrganization((int)((NamObjectItem)cmbOrganization.SelectedItem).Value);
      LoadOrganization((int)((NamObjectItem)cmbOrganization.SelectedItem).Value);
    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (tabControl1.SelectedTab != tpQuery) return;
      if (e.KeyCode == Keys.F5)
      {
        queryExcelControl1.ExecuteQuery();
        e.Handled = true;
      }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.Save();
    }

    private void btnTestButton_Click(object sender, EventArgs e)
    {
      /*
      SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=IC;Integrated Security=True");
      connection.Open();
      string cmdString = @"select 
CAST(ie.ieincidentid AS VarChar(40)) + ', ' + CAST(ie.ieProjectID AS VARCHAR(40)) + ', ' + CAST(ie.ieventid AS VARCHAR(40)) AS TicketID,
isnull(mb.subject,'') as Name, datecreated as DateCreated, -1 as CreatorID, 'Email' as ActionType, null as DateStarted, null as TimeSpent, mbb.mailboxbodytext as Description
from incidentevent ie
left join mailbox mb on mb.mailid = ie.mailitemid
left join mailboxbody mbb on mb.mailid = mbb.mailid
where (rtrim(isnull(mb.Subject,'')) <> '')
 or not (mbb.mailboxbodytext  is null)

";

      SqlDataAdapter adapter = new SqlDataAdapter(cmdString, connection);
      DataTable table = new DataTable();
      adapter.Fill(table);

      int orgid = (int)((NamObjectItem)cmbOrganization.SelectedItem).Value;
      Users users = new Users(LoginSession.LoginUser);
      users.LoadByOrganizationID(orgid, false);

      Tickets tickets = new Tickets(LoginSession.LoginUser);
      tickets.LoadByOrganizationID(orgid);

      ActionTypes actionTypes = new ActionTypes(LoginSession.LoginUser);
      actionTypes.LoadAllPositions(orgid);


      Actions actions = new Actions(LoginSession.LoginUser);

      foreach (DataRow row in table.Rows)
      {
        Ticket ticket = tickets.FindByImportID(row["TicketID"].ToString().Trim());
        if (ticket == null) continue;

        User creator = users.FindByImportID(row["CreatorID"].ToString().Trim());
        int creatorID = creator == null ? LoginSession.LoginUser.UserID : creator.UserID;

        TeamSupport.Data.Action action = actions.AddNewAction();

        SystemActionType sysActionType = GetSystemActionTypeID(row["ActionType"].ToString().Trim());

        action.ActionTypeID = sysActionType == SystemActionType.Custom ? GetActionTypeID(row["ActionType"].ToString().Trim(), actionTypes) : null;
        action.CreatorID = creatorID;
        action.DateCreated = (DateTime)GetDBDate(row["DateCreated"].ToString().Trim(), false);
        action.DateModified = DateTime.Now;
        action.DateStarted = GetDBDate(row["DateStarted"].ToString().Trim(), true);
        action.Description = row["Description"].ToString().Trim();
        action.IsVisibleOnPortal = false;
        action.ModifierID = LoginSession.LoginUser.UserID;
        action.Name = row["Name"].ToString().Trim();
        if (action.Name.Length > 499) action.Name = action.Name.Substring(0, 499);
        action.SystemActionTypeID = sysActionType;
        action.TicketID = ticket.TicketID;
        action.TimeSpent = GetDBInt(row["TimeSpent"], true);
      }

      actions.BulkSave();
      MessageBox.Show(table.Rows.Count.ToString());

      

      /*
      DataSet dataSet = new DataSet();
      dataSet.ReadXml(@"U:\Development\TeamSupport\Imports\Integrated Clinical\MailBoxBody2.xml");
      SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=IC;Integrated Security=True");
      connection.Open();

      DataTable table = new DataTable();
      table.Columns.Add("ProjectID", Type.GetType("System.Int32"));
      table.Columns.Add("MailID", Type.GetType("System.Int32"));
      table.Columns.Add("MailBoxBodyText", Type.GetType("System.String"));
      table.ReadXml(@"U:\Development\TeamSupport\Imports\Integrated Clinical\MailBoxBody2.xml");

      SqlBulkCopy copy = new SqlBulkCopy(connection);
      copy.DestinationTableName = "MailBoxBody";
      copy.WriteToServer(dataSet.Tables[0]);
      table.AcceptChanges();
     
      */

    }

    private SystemActionType GetSystemActionTypeID(string name)
    {
      SystemActionType result = SystemActionType.Custom;
      switch (name.ToLower().Trim())
      {
        case "description": result = SystemActionType.Description; break;
        case "resolution": result = SystemActionType.Resolution; break;
        case "pingupdate": result = SystemActionType.PingUpdate; break;
        case "email": result = SystemActionType.Email; break;
        default:
          break;
      }
      return result;
    }

    private int? GetActionTypeID(string name, ActionTypes actionTypes)
    {
      ActionType actionType = actionTypes.FindByName(name);
      if (actionType == null)
        return null;
      else
        return actionType.ActionTypeID;
    }

    private DateTime? GetDBDate(object o, bool allowNull)
    {
      DateTime? result = allowNull ? null : (DateTime?)DateTime.Now;

      try
      {
        if (o.ToString().Trim() != "") result = DateTime.Parse(o.ToString().Trim());
      }
      catch (Exception)
      {
      }

      return result;
    }

    private int? GetDBInt(object o, bool allowNull)
    {
      int? result = allowNull ? null : (int?)0;

      try
      {
        if (o.ToString().Trim() != "") result = int.Parse(o.ToString().Trim());
      }
      catch (Exception)
      {
      }

      return result;
    }

    private string GetDBString(object o, int maxLength, bool allowNull)
    {
      string result = o.ToString().Trim();
      if (allowNull && result == "") return null;
      if (result.Length > maxLength) result = result.Substring(0, maxLength);
      return result;
    }

 

  }
}
