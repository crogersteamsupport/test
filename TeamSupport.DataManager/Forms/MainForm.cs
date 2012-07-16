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
using OpenPop.Common.Logging;
using OpenPop.Mime;
using OpenPop.Mime.Decode;
using OpenPop.Mime.Header;
using OpenPop.Pop3;
using System.IO;

namespace TeamSupport.DataManager
{
  public partial class MainForm : Form
  {
  
    public MainForm()
    {
      InitializeComponent();
      //ForAspNet.POP3.License.LicenseKey = "Gk11cm9jIFN5c3RlbXMsIEluYy4gKFcyNzQpAQAAAAEAAAD/Pzf0dSjKK0y+sCpu2UbqXtI36PB0YjN1dnd0wwMsiHXhwI7S7tI9Z3jaJDIohYejP1t5FaqL3w==";
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
      //Importer.SetTypeGuessRowsForExcelImport();
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

    private Attachments _attachments = null;

    private void btnTestButton_Click(object sender, EventArgs e)
    {

      Pop3Client pop = new Pop3Client();
      pop.Connect("pop.gmail.com", 995, true);
      pop.Authenticate("recent:tickets@teamsupport.com", "Muroc2008!");
      int count = pop.GetMessageCount();
      DateTime startDate = new DateTime(2012, 4, 23, 20, 0, 0);
      DateTime endDate = new DateTime(2012, 4, 24, 10, 0, 0);
      for (int i = count; i > 0; i--)
			{
        try
        {
          MessageHeader header = pop.GetMessageHeaders(i);
          if (header.DateSent > startDate && header.DateSent < endDate) continue;

          OpenPop.Mime.Message message = pop.GetMessage(i);

          foreach (MessagePart attachment in message.FindAllAttachments())
	        {
		        ProcessAttachment(attachment.FileName, attachment.Body);
	        }
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
			}
      MessageBox.Show("Done");
    }

    private void ProcessAttachment(string fileName, byte[] bytes)
    {
      Attachment attachment = FindAttachment(fileName);
      if (attachment != null)
      {
        string path = string.Format(@"\\MUROC-WEB1\TSData\Organizations\{0}\Actions\{1}\", attachment.OrganizationID, attachment.RefID);
        //string path = string.Format(@"c:\TSData2\Organizations\{0}\Actions\{1}\", attachment.OrganizationID, attachment.RefID);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        path = Path.Combine(path, fileName);
        if (!File.Exists(path))
        {
          File.WriteAllBytes(path, bytes);
          attachment.Path = path;
          attachment.Collection.Save();
        }
      }
    }

    private Attachment FindAttachment(string fileName)
    {
      if (_attachments == null) 
      { 
        _attachments = new Attachments(LoginSession.LoginUser);
        _attachments.TempLoadFix();
      }

      Attachment result = null;

      foreach (Attachment attachment in _attachments)
      {
        if (attachment.FileName.ToLower().Trim() == fileName.ToLower().Trim())
        {
          if (result != null) return null;
          result = attachment;
        }
      }
      return result;
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
